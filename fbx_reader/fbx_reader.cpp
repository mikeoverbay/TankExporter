// fbx_reader.cpp
// Self-contained FBX binary reader DLL.
// Parses Kaydara FBX Binary format (v6100-v7400) produced by Aspose.3D.
// No external dependencies — DEFLATE implemented from scratch.
//
// Transform un-inversions applied (matching Aspose.3D exporter quirks):
//   normal.x  : multiply by -1  (exporter negated X)
//   UV0.v     : 1.0f - v        (exporter wrote Y = 1-v)
//   UV2.v2    : -v2             (exporter wrote Y = -v2)
//   bone byte : round(f * 255)  (exporter stored bytes as 0-1 floats in color)

#include "pch.h"
#define FBX_READER_EXPORTS
#include "fbx_reader.h"

#include <windows.h>
#include <io.h>
#include <fcntl.h>
#include <iostream>

#include <vector>
#include <string>
#include <array>
#include <unordered_map>
#include <cstring>
#include <cstdio>
#include <cmath>
#include <algorithm>
#include <cassert>
// ============================================================
//  1.  DEFLATE decompressor (no zlib dependency)
// ============================================================

// BitReader — reads individual bits or bytes from a raw memory buffer.
// DEFLATE uses LSB-first bit ordering: bit 0 of each byte is read first.
// Tracks both the current byte position (pos) and bit index within that byte (bit).
struct BitReader {
    const uint8_t* data;
    size_t         size;
    size_t         pos;   // byte position
    int            bit;   // bit index within current byte (0-7)

    // Attach the reader to a buffer. Resets position to the start.
    void init(const uint8_t* d, size_t s) {
        data = d; size = s; pos = 0; bit = 0;
    }

    // Read one bit, LSB-first within each byte.
    // Returns 0 if past the end of the buffer (safe overread).
    int read_bit() {
        if (pos >= size) return 0;
        int b = (data[pos] >> bit) & 1;
        if (++bit == 8) { bit = 0; ++pos; }
        return b;
    }

    // Read n bits (up to 32), assembling them LSB-first into an integer.
    uint32_t read_bits(int n) {
        uint32_t v = 0;
        for (int i = 0; i < n; ++i) v |= (read_bit() << i);
        return v;
    }

    // Discard any remaining bits in the current byte to align to the next byte boundary.
    // Used before stored-block reads, which require byte alignment per RFC 1951.
    void byte_align() {
        if (bit != 0) { bit = 0; ++pos; }
    }

    // Read one full byte, aligning first if mid-byte. Returns 0 on overread.
    uint8_t read_byte() {
        byte_align();
        if (pos >= size) return 0;
        return data[pos++];
    }

    // Read a 16-bit little-endian value as two consecutive bytes.
    // Used for the LEN/NLEN fields in stored DEFLATE blocks.
    uint16_t read_u16_le() {
        uint8_t lo = read_byte();
        uint8_t hi = read_byte();
        return (uint16_t)(lo | (hi << 8));
    }
};

// Huffman — canonical Huffman decoder using puff-style table-free decode.
// Works LSB-first to match DEFLATE's bit ordering.
// Stores only the count of codes per length and a flat sorted symbol array —
// no per-code lookup table needed.
struct Huffman {
    static const int MAXBITS = 15;
    int count[MAXBITS + 1]; // count[n] = number of codes of length n
    int symbol[288];        // symbols ordered by (length, value) — the canonical order

    // Build the decoder from a lengths[] array of 'n' symbols.
    // lengths[i] is the bit-length of symbol i; 0 means the symbol is unused.
    // Returns false if the lengths describe an over-subscribed or empty tree.
    bool build(const int* lengths, int n) {
        memset(count, 0, sizeof(count));
        for (int i = 0; i < n; ++i)
            if (lengths[i] > 0) count[lengths[i]]++;

        // Verify it's a valid tree: each level must not over-subscribe the code space.
        int left = 1;
        for (int len = 1; len <= MAXBITS; ++len) {
            left <<= 1;
            left -= count[len];
            if (left < 0) return false;
        }

        // Fill the symbol array in canonical order (sorted by length, then value).
        // offs[len] is the next insertion position for codes of that length.
        int offs[MAXBITS + 1];
        offs[1] = 0;
        for (int len = 1; len < MAXBITS; ++len)
            offs[len + 1] = offs[len] + count[len];

        for (int sym = 0; sym < n; ++sym)
            if (lengths[sym] > 0)
                symbol[offs[lengths[sym]]++] = sym;

        return true;
    }

    // Decode one symbol by reading bits one at a time from br.
    // Uses the puff algorithm: walks code lengths comparing accumulated code
    // against the first code value at each length. Returns -1 on error.
    int decode(BitReader& br) const {
        int code = 0, first = 0, index = 0;
        for (int len = 1; len <= MAXBITS; ++len) {
            code |= br.read_bit();
            int c = count[len];
            if ((code - c) < first) {
                return symbol[index + (code - first)];
            }
            index += c;
            first = (first + c) << 1;
            code <<= 1;
        }
        return -1; // no valid symbol found — corrupt stream
    }
};

// Build the fixed (pre-defined) Huffman trees specified by RFC 1951 §3.2.6.
// Used for DEFLATE type-1 blocks which skip the dynamic tree header.
// llen receives the literal/length tree (288 symbols, mixed 7/8/9-bit codes).
// ldist receives the distance tree (32 symbols, all 5-bit codes).
static void make_fixed_huffman(Huffman& llen, Huffman& ldist) {
    int ll[288], ld[32];
    for (int i = 0; i <= 143; ++i) ll[i] = 8;
    for (int i = 144; i <= 255; ++i) ll[i] = 9;
    for (int i = 256; i <= 279; ++i) ll[i] = 7;
    for (int i = 280; i <= 287; ++i) ll[i] = 8;
    for (int i = 0; i <= 31; ++i) ld[i] = 5;
    llen.build(ll, 288);
    ldist.build(ld, 32);
}

void OpenConsole() {
    // Only create a console if one doesn't exist
    if (AllocConsole()) {
        FILE* fDummy;
        freopen_s(&fDummy, "CONOUT$", "w", stdout);
        freopen_s(&fDummy, "CONOUT$", "w", stderr);
        freopen_s(&fDummy, "CONIN$", "r", stdin);
        std::clog.clear();
        std::clog << "[FBX Reader] Debug Console Attached\n";
    }
}
// DEFLATE back-reference decode tables (RFC 1951 §3.2.5).
// Length symbols 257-285 are decoded as: LENGTH_BASE[sym-257] + read_bits(LENGTH_EXTRA[sym-257])
// Distance symbols 0-29 are decoded as:  DIST_BASE[sym]       + read_bits(DIST_EXTRA[sym])
// CLCL_ORDER is the permutation used to read code-length-code lengths in dynamic blocks.
static const int LENGTH_BASE[29] = {
    3,4,5,6,7,8,9,10,11,13,15,17,19,23,27,31,
    35,43,51,59,67,83,99,115,131,163,195,227,258
};
static const int LENGTH_EXTRA[29] = {
    0,0,0,0,0,0,0,0,1,1,1,1,2,2,2,2,
    3,3,3,3,4,4,4,4,5,5,5,5,0
};
static const int DIST_BASE[30] = {
    1,2,3,4,5,7,9,13,17,25,33,49,65,97,129,193,
    257,385,513,769,1025,1537,2049,3073,4097,6145,
    8193,12289,16385,24577
};
static const int DIST_EXTRA[30] = {
    0,0,0,0,1,1,2,2,3,3,4,4,5,5,6,6,
    7,7,8,8,9,9,10,10,11,11,12,12,13,13
};
static const int CLCL_ORDER[19] = {
    16,17,18,0,8,7,9,6,10,5,11,4,12,3,13,2,14,1,15
};

// Decompress one DEFLATE block from br and append output bytes to 'out'.
// Sets 'last' to true if the block header's BFINAL bit is set (this is the final block).
// Handles all three block types:
//   type 0 — stored (raw bytes, byte-aligned, with LEN/NLEN verification)
//   type 1 — fixed Huffman trees (built by make_fixed_huffman)
//   type 2 — dynamic Huffman trees (lengths encoded in the block header itself)
// Back-references are resolved by copying from already-written output bytes.
// Returns false on any format error.
static bool inflate_block(BitReader& br, std::vector<uint8_t>& out, bool& last) {
    last = (bool)br.read_bits(1);
    int type = (int)br.read_bits(2);

    if (type == 0) {
        // Stored block
        br.byte_align();
        uint16_t len = br.read_u16_le();
        uint16_t nlen = br.read_u16_le();
        if ((len ^ nlen) != 0xFFFF) return false;
        for (int i = 0; i < len; ++i) out.push_back(br.read_byte());
        return true;
    }

    Huffman llen, ldist;
    if (type == 1) {
        make_fixed_huffman(llen, ldist);
    }
    else if (type == 2) {
        // Dynamic Huffman
        int hlit = (int)br.read_bits(5) + 257;
        int hdist = (int)br.read_bits(5) + 1;
        int hclen = (int)br.read_bits(4) + 4;

        int cl_lengths[19] = {};
        for (int i = 0; i < hclen; ++i)
            cl_lengths[CLCL_ORDER[i]] = (int)br.read_bits(3);

        Huffman cl;
        cl.build(cl_lengths, 19);

        // Decode code length array
        int lengths[320] = {};
        int ntotal = hlit + hdist, i = 0;
        while (i < ntotal) {
            int sym = cl.decode(br);
            if (sym < 0) return false;
            if (sym < 16) {
                lengths[i++] = sym;
            }
            else if (sym == 16) {
                int rep = (int)br.read_bits(2) + 3;
                int prev = (i > 0) ? lengths[i - 1] : 0;
                for (int k = 0; k < rep && i < ntotal; ++k) lengths[i++] = prev;
            }
            else if (sym == 17) {
                int rep = (int)br.read_bits(3) + 3;
                for (int k = 0; k < rep && i < ntotal; ++k) lengths[i++] = 0;
            }
            else {
                int rep = (int)br.read_bits(7) + 11;
                for (int k = 0; k < rep && i < ntotal; ++k) lengths[i++] = 0;
            }
        }
        llen.build(lengths, hlit);
        ldist.build(lengths + hlit, hdist);
    }
    else {
        return false; // reserved
    }

    // Decode literals/lengths/distances
    for (;;) {
        int sym = llen.decode(br);
        if (sym < 0)   return false;
        if (sym == 256) break; // end of block
        if (sym < 256) {
            out.push_back((uint8_t)sym);
        }
        else {
            int li = sym - 257;
            int len = LENGTH_BASE[li] + (int)br.read_bits(LENGTH_EXTRA[li]);
            int di = ldist.decode(br);
            if (di < 0) return false;
            int dist = DIST_BASE[di] + (int)br.read_bits(DIST_EXTRA[di]);
            int back = (int)out.size() - dist;
            for (int k = 0; k < len; ++k)
                out.push_back(out[back + k]);
        }
    }
    return true;
}

// Decompress a complete raw DEFLATE stream (no zlib header).
// Calls inflate_block repeatedly until the final block is reached.
// 'in' points to the first DEFLATE byte; 'in_size' is its byte length.
// Decompressed bytes are appended to 'out'. Returns false on error.
static bool inflate_stream(const uint8_t* in, size_t in_size, std::vector<uint8_t>& out) {
    BitReader br;
    br.init(in, in_size);
    bool last = false;
    while (!last) {
        if (!inflate_block(br, out, last)) return false;
    }
    return true;
}

// Decompress a zlib-wrapped DEFLATE stream (RFC 1950).
// Skips the 2-byte zlib header (CMF + FLG) and the 4-byte Adler-32 checksum footer,
// then hands the inner DEFLATE stream to inflate_stream.
// FBX binary format uses this exact wrapping for compressed array properties (encoding=1).
static bool zlib_uncompress(const uint8_t* in, size_t in_size, std::vector<uint8_t>& out) {
    if (in_size < 6) return false;
    return inflate_stream(in + 2, in_size - 6, out);
}

// ============================================================
//  2.  FBX binary parser
// ============================================================

// ByteReader — sequential byte-level reader over a flat memory buffer.
// Used for the FBX binary structure (node records, property headers, raw scalars).
// All multi-byte values in FBX are stored little-endian.
// memcpy is used for all multi-byte reads to avoid strict-aliasing UB.
struct ByteReader {
    const uint8_t* data;
    size_t         size;
    size_t         pos;

    // True as long as pos has not overrun the buffer.
    bool ok() const { return pos <= size; }
    // True if at least n bytes remain from the current position.
    bool can_read(size_t n) const { return pos + n <= size; }

    uint8_t  read_u8() { uint8_t v = data[pos]; pos += 1; return v; }
    uint16_t read_u16() { uint16_t v; memcpy(&v, data + pos, 2); pos += 2; return v; }
    uint32_t read_u32() { uint32_t v; memcpy(&v, data + pos, 4); pos += 4; return v; }
    uint64_t read_u64() { uint64_t v; memcpy(&v, data + pos, 8); pos += 8; return v; }
    float    read_f32() { float    v; memcpy(&v, data + pos, 4); pos += 4; return v; }
    double   read_f64() { double   v; memcpy(&v, data + pos, 8); pos += 8; return v; }

    // Read exactly n bytes as a std::string (used for node names and string properties).
    std::string read_string(size_t n) {
        std::string s((char*)data + pos, n);
        pos += n;
        return s;
    }
    // Advance past n bytes without reading them.
    void skip(size_t n) { pos += n; }
    // Jump to an absolute byte offset (used to seek to end_offset after reading a node).
    void seek(size_t p) { pos = p; }
};

// FbxProp — one property value attached to an FBX node record.
// The 'type' character identifies which field is valid (matches the FBX binary type code):
//   'Y' = int16,  'C' = bool,  'I' = int32,  'L' = int64,  'F' = float32,  'D' = float64
//   'S' = string, 'R' = raw bytes
//   'f' = float32 array,  'd' = float64 array,  'i' = int32 array,  'l' = int64 array
struct FbxProp {
    char type; // Y S B I L F D f d i l R S-string  C-bool
    // scalars
    union {
        int16_t  Y;
        bool     C;
        int32_t  I;
        int64_t  L;
        float    F;
        double   D;
    } s;
    std::string str;

    // arrays
    std::vector<double>  arr_d; // D or d
    std::vector<float>   arr_f; // F or f
    std::vector<int32_t> arr_i; // I or i
    std::vector<int64_t> arr_l; // L or l
};

// FbxNode — one record in the FBX tree.
// Each node has a name (e.g. "Geometry", "Objects", "LayerElementNormal"),
// zero or more typed properties, and zero or more child nodes.
// The tree is built recursively by read_node().
struct FbxNode {
    std::string            name;
    std::vector<FbxProp>   props;
    std::vector<FbxNode>   children;
};

// Read an FBX typed array property (types 'f','d','i','l') from br into 'out'.
// Array header: arr_len (u32) + encoding (u32) + comp_len (u32).
//   encoding 0 = raw: arr_len * sizeof(T) bytes follow directly.
//   encoding 1 = zlib: comp_len compressed bytes follow; zlib_uncompress expands them.
// T must be float, double, int32_t, or int64_t (matching the FBX array element size).
template<typename T>
static bool read_typed_array(ByteReader& br, std::vector<T>& out) {
    uint32_t arr_len = br.read_u32();
    uint32_t encoding = br.read_u32();
    uint32_t comp_len = br.read_u32();

    if (encoding == 0) {
        out.resize(arr_len);
        if (!br.can_read(arr_len * sizeof(T))) return false;
        memcpy(out.data(), br.data + br.pos, arr_len * sizeof(T));
        br.pos += arr_len * sizeof(T);
    }
    else {
        if (!br.can_read(comp_len)) return false;
        std::vector<uint8_t> raw;
        raw.reserve(arr_len * sizeof(T));
        if (!zlib_uncompress(br.data + br.pos, comp_len, raw)) return false;
        br.pos += comp_len;
        if (raw.size() < arr_len * sizeof(T)) return false;
        out.resize(arr_len);
        memcpy(out.data(), raw.data(), arr_len * sizeof(T));
    }
    return true;
}

// Read one FBX property from br into p.
// Reads the type byte first, then dispatches to the correct scalar or array reader.
// String/raw ('S'/'R') properties are length-prefixed (u32 byte count).
// Array properties delegate to read_typed_array<T>.
// Returns false for an unrecognised type code.
static bool read_prop(ByteReader& br, FbxProp& p) {
    p.type = (char)br.read_u8();
    switch (p.type) {
    case 'Y': p.s.Y = (int16_t)br.read_u16(); break;
    case 'C': p.s.C = (bool)br.read_u8();     break;
    case 'I': p.s.I = (int32_t)br.read_u32(); break;
    case 'L': p.s.L = (int64_t)br.read_u64(); break;
    case 'F': p.s.F = br.read_f32();           break;
    case 'D': p.s.D = br.read_f64();           break;
    case 'f': return read_typed_array(br, p.arr_f);
    case 'd': return read_typed_array(br, p.arr_d);
    case 'i': return read_typed_array(br, p.arr_i);
    case 'l': return read_typed_array(br, p.arr_l);
    case 'R':
    case 'S': {
        uint32_t len = br.read_u32();
        if (!br.can_read(len)) return false;
        p.str = br.read_string(len);
        break;
    }
    default: return false;
    }
    return true;
}

// Read one FBX node record (and all its children recursively) from br into 'node'.
// Node record layout:
//   v < 7500:  end_offset(u32), prop_count(u32), prop_length(u32), name_len(u8), name[name_len]
//   v >= 7500: end_offset(u64), prop_count(u64), prop_length(u64), name_len(u8), name[name_len]
// A null sentinel (all-zero header + name_len=0) signals the end of a child list.
// After reading properties and recursing into children, br is seeked to end_offset
// to skip any unrecognised content within the node's declared bounds.
// Returns true on both a normal node and a null sentinel; caller checks node.name.empty().
static bool read_node(ByteReader& br, FbxNode& node, bool v7500) {
    uint64_t end_offset, prop_count, prop_length;

    if (v7500) {
        end_offset = br.read_u64();
        prop_count = br.read_u64();
        prop_length = br.read_u64();
    }
    else {
        end_offset = br.read_u32();
        prop_count = br.read_u32();
        prop_length = br.read_u32();
    }

    uint8_t name_len = br.read_u8();
    if (end_offset == 0 && prop_count == 0 && prop_length == 0 && name_len == 0)
        return true; // null sentinel — signal caller to stop

    node.name = br.read_string(name_len);

    // Read properties
    node.props.resize((size_t)prop_count);
    for (size_t i = 0; i < (size_t)prop_count; ++i) {
        if (!read_prop(br, node.props[i])) return false;
    }

    // Read children until null sentinel
    while (br.pos < (size_t)end_offset) {
        FbxNode child;
        if (!read_node(br, child, v7500)) return false;
        if (child.name.empty()) break; // null sentinel consumed
        node.children.push_back(std::move(child));
    }

    br.seek((size_t)end_offset);
    return true;
}

// Parse a complete FBX binary file into a tree of FbxNodes rooted at 'root'.
// Verifies the 23-byte magic header "Kaydara FBX Binary  \x00\x1a\x00",
// reads the 4-byte version number (e.g. 7400), then reads top-level nodes
// until a null sentinel or end of file.
// Sets fbx_version so the caller knows which offset width was used.
// Returns false if the magic header does not match (i.e. not a binary FBX file).
static bool parse_fbx(const uint8_t* data, size_t size, FbxNode& root, int& fbx_version) {
    // Check magic
    static const char MAGIC[] = "Kaydara FBX Binary  \x00\x1a\x00";
    if (size < 27) return false;
    if (memcmp(data, MAGIC, 23) != 0) return false;

    ByteReader br;
    br.data = data; br.size = size; br.pos = 23;
    fbx_version = (int)br.read_u32();
    bool v7500 = (fbx_version >= 7500);

    root.name = "root";
    while (br.pos < size) {
        // Peek for sentinel
        size_t sentinel_size = v7500 ? 25 : 13;
        if (br.pos + sentinel_size > size) break;

        FbxNode child;
        if (!read_node(br, child, v7500)) break;
        if (child.name.empty()) break;
        root.children.push_back(std::move(child));
    }
    return true;
}

// ============================================================
//  3.  Scene data structures
// ============================================================

// build_trs_matrix — build a 16-element column-major (OpenGL) 4x4 matrix from
// translation (tx,ty,tz), Euler XYZ rotation in degrees (rx,ry,rz), and scale (sx,sy,sz).
// Rotation order: R = Rz * Ry * Rx  (X applied first, matching FBX default EulerXYZ).
// Output layout matches DecomposeMatrix() in get_FBX.vb:
//   out[col*4+row]  →  column-major, translation at out[12..14].
static void build_trs_matrix(
    double tx, double ty, double tz,
    double rx, double ry, double rz,
    double sx, double sy, double sz,
    double out[16])
{
    const double DEG2RAD = 3.14159265358979323846 / 180.0;
    double cx = cos(rx * DEG2RAD), sx_ = sin(rx * DEG2RAD);
    double cy = cos(ry * DEG2RAD), sy_ = sin(ry * DEG2RAD);
    double cz = cos(rz * DEG2RAD), sz_ = sin(rz * DEG2RAD);

    // R = Rz * Ry * Rx
    double r00 = cz * cy;
    double r10 = sz_ * cy;
    double r20 = -sy_;
    double r01 = cz * sy_ * sx_ - sz_ * cx;
    double r11 = sz_ * sy_ * sx_ + cz * cx;
    double r21 = cy * sx_;
    double r02 = cz * sy_ * cx + sz_ * sx_;
    double r12 = sz_ * sy_ * cx - cz * sx_;
    double r22 = cy * cx;

    // Column-major storage: out[col*4 + row]
    out[0] = r00 * sx;  out[1] = r10 * sx;  out[2] = r20 * sx;  out[3] = 0.0;
    out[4] = r01 * sy;  out[5] = r11 * sy;  out[6] = r21 * sy;  out[7] = 0.0;
    out[8] = r02 * sz;  out[9] = r12 * sz;  out[10] = r22 * sz;  out[11] = 0.0;
    out[12] = tx;      out[13] = ty;      out[14] = tz;       out[15] = 1.0;
}

// find_child — return the first child of 'n' whose name matches, or nullptr.
// Used throughout the scene extractor to navigate the FBX node tree by name.
static const FbxNode* find_child(const FbxNode& n, const char* name) {
    for (const auto& c : n.children)
        if (c.name == name) return &c;
    return nullptr;
}
// Mutable version of find_child — returns a non-const pointer for in-place modification.
static FbxNode* find_child_m(FbxNode& n, const char* name) {
    for (auto& c : n.children)
        if (c.name == name) return &c;
    return nullptr;
}

// LayerElem — holds the raw data for one FBX layer element (normals, UVs, or vertex colors).
// FBX stores per-vertex attributes separately from positions, in "LayerElement" sub-nodes.
// 'mapping' controls how indices map to data entries:
//   "ByControlPoint"  — one data entry per control point (base vertex)
//   "ByPolygonVertex" — one data entry per polygon-vertex occurrence (expanded)
// 'ref' controls whether data is accessed directly or via an indirection table:
//   "Direct"          — data[idx * components .. +components]
//   "IndexToDirect"   — data[index[idx] * components .. +components]
// 'components' is 3 for normals/colors (XYZ or RGBA) and 2 for UVs.
struct LayerElem {
    std::string          type;   // "LayerElementNormal" etc.
    std::string          mapping; // "ByControlPoint" / "ByPolygonVertex"
    std::string          ref;     // "Direct" / "IndexToDirect"
    std::vector<double>  data;    // x,y,z or u,v interleaved
    std::vector<int32_t> index;   // for IndexToDirect
    int                  components = 3; // 2 for UV
};

// RawMesh — intermediate storage for one FBX Geometry object before baking.
// Holds the raw control-point array, polygon index list, and separate layer elements
// exactly as read from the file. build_mesh() converts this into a BakedMesh.
struct RawMesh {
    int64_t              geom_id = 0;
    std::string          name;
    std::string          diffuse_tex;
    std::string          normal_tex;
    std::vector<double>  cp;          // control points: x,y,z per vertex
    std::vector<int32_t> poly_idx;    // FBX polygon indices (negative = end of face)
    LayerElem            normals;
    LayerElem            uv0;
    LayerElem            uv1;
    std::vector<LayerElem> colors;    // up to 4 color channels (bone data)
    double               matrix[16];  // column-major 4x4 local transform (identity until resolved)
    // Custom properties written by export_aspose_fbx via base.SetProperty()
    std::string          primitive_table; // e.g. "hull/lod0/indices" — section name in .primitives_processed
    int                  primitive_index = -1; // 1-based item slot index at export time
    RawMesh() {
        // Identity matrix
        memset(matrix, 0, sizeof(matrix));
        matrix[0] = 1; matrix[5] = 1; matrix[10] = 1; matrix[15] = 1;
    }
};

// BakedMesh — final per-mesh data ready for the C API to serve.
// Vertices are deduplicated FbxVertex structs; faces are triangle indices.
// Aspose.3D export transforms have already been reversed in the vertex data.
struct BakedMesh {
    std::string              name;
    std::string              diffuse_tex;
    std::string              normal_tex;
    std::vector<FbxVertex>   verts;
    std::vector<FbxFace>     faces;
    int                      color_ch = 0;
    bool                     has_uv2 = false;
    double                   matrix[16]; // column-major 4x4 local transform from Model node
    // Custom properties — round-tripped from export_aspose_fbx via Aspose Node.SetProperty()
    std::string              primitive_table; // section name in .primitives_processed file
    int                      primitive_index = -1; // 1-based slot index at export time
    BakedMesh() {
        memset(matrix, 0, sizeof(matrix));
        matrix[0] = 1; matrix[5] = 1; matrix[10] = 1; matrix[15] = 1;
    }
};

// ============================================================
//  4.  Layer element extraction helpers
// ============================================================

// fill_layer_elem — populate a LayerElem from an FBX LayerElement* child node.
// Reads MappingInformationType, ReferenceInformationType, the data array
// (node named "Normals", "UV", "Colors", or "Tangents"), and the optional
// index array ("NormalsIndex", "UVIndex", "ColorIndex").
// 'components' must be 2 for UV channels or 3/4 for normals/colors.
// Accepts both 'd' (double) and 'f' (float) array types, converting float to double.
static void fill_layer_elem(const FbxNode& node, LayerElem& elem, int components) {
    elem.components = components;
    for (const auto& c : node.children) {
        if (c.name == "MappingInformationType" && !c.props.empty())
            elem.mapping = c.props[0].str;
        else if (c.name == "ReferenceInformationType" && !c.props.empty())
            elem.ref = c.props[0].str;
        else if ((c.name == "Normals" || c.name == "UV" ||
            c.name == "Colors" || c.name == "Tangents") &&
            !c.props.empty()) {
            // 'd' array
            const FbxProp& p = c.props[0];
            if (p.type == 'd') elem.data = p.arr_d;
            else if (p.type == 'f') {
                elem.data.resize(p.arr_f.size());
                for (size_t i = 0; i < p.arr_f.size(); ++i) elem.data[i] = p.arr_f[i];
            }
        }
        else if ((c.name == "NormalsIndex" || c.name == "UVIndex" ||
            c.name == "ColorIndex") && !c.props.empty()) {
            const FbxProp& p = c.props[0];
            if (p.type == 'i') elem.index = p.arr_i;
        }
    }
}

// sample_layer — read one data entry from a LayerElem for a given polygon-vertex.
// ctrl_idx = the control point index for this polygon-vertex (used for ByControlPoint mapping).
// pv_idx   = the running polygon-vertex counter (used for ByPolygonVertex mapping).
// Handles the IndexToDirect indirection automatically.
// Writes le.components values into out[0..components-1].
// Returns false if the layer has no data or the resolved index is out of range.
static bool sample_layer(const LayerElem& le, int ctrl_idx, int pv_idx, double* out) {
    if (le.data.empty()) return false;
    int idx = 0;
    // Fix 5: handle AllSame mapping (every polygon-vertex uses element 0).
    // Previously it fell through to ByPolygonVertex, reading pv_idx into a
    // 1-element array and returning false → UV sampled as 0 → dedup collapsed
    // too many vertices → wrong (too-low) vert count for meshes with AllSame UV.
    if (le.mapping == "ByControlPoint") {
        idx = ctrl_idx;
    }
    else if (le.mapping == "AllSame") {
        idx = 0;
    }
    else {
        // ByPolygonVertex (and any other future mapping type)
        idx = pv_idx;
    }
    if (le.ref == "IndexToDirect") {
        if (idx < 0 || idx >= (int)le.index.size()) return false;
        idx = le.index[idx];
    }
    int base = idx * le.components;
    if (base + le.components > (int)le.data.size()) return false;
    for (int i = 0; i < le.components; ++i) out[i] = le.data[base + i];
    return true;
}
#define DEBUG_OUTPUT(fmt, ...) do { \
    wchar_t _buf[512]; \
    swprintf_s(_buf, L"[FBX] " fmt, __VA_ARGS__); \
    OutputDebugString(_buf); \
} while(0)

// ============================================================
//  5.  Mesh baking (polygon-vertex expansion + deduplication)
// ============================================================

// build_mesh — convert a RawMesh into a BakedMesh with a flat deduplicated vertex array.
//
// Step 1: Walk raw.poly_idx (the FBX polygon vertex index list).
//         Negative values mark the final vertex of each face (~value gives the real index).
//         For each polygon-vertex, sample all layer elements (normal, UV0, UV1, colors)
//         and apply the Aspose.3D export transform inversions:
//           normal.x  = -nx        (exporter negated X)
//           uv0.v     = 1.0f - Y   (exporter wrote Y = 1 - v)
//           uv1.v2    = -Y         (exporter wrote Y = -v2)
//           color byte = round(f*255) (exporter stored bone bytes as 0-1 floats)
//         Quads and n-gons are fan-triangulated (v0, vk, vk+1).
//
// Step 2: Deduplicate the expanded polygon-vertex list using an unordered_map
//         keyed by a bit-exact memcmp of the PV struct, producing a minimal
//         vertex array and updating the face index list to match.
static void build_mesh(const RawMesh& raw, BakedMesh& baked) {
    baked.name = raw.name;
    baked.diffuse_tex = raw.diffuse_tex;
    baked.normal_tex = raw.normal_tex;
    baked.has_uv2 = !raw.uv1.data.empty();
    baked.color_ch = (int)raw.colors.size();
    memcpy(baked.matrix, raw.matrix, 16 * sizeof(double));
    baked.primitive_table = raw.primitive_table;
    baked.primitive_index = raw.primitive_index;

    // Walk polygon index list
    // Collect per-polygon-vertex data then build a deduplicated vertex list
    // Key: pack float data as bit-identical to detect seam splits
    struct PV {
        float x, y, z, nx, ny, nz, u, v, u2, v2;
        uint8_t i1, i2, i3, i4, w1, w2, w3, w4;
    };

    std::vector<PV> pv_list;
    std::vector<int> polygon_verts; // indices into pv_list for current face
    int pv_idx = 0; // running polygon-vertex counter

    auto byte_clamp = [](double f) -> uint8_t {
        int v = (int)round(f * 255.0);
        if (v < 0) v = 0;
        if (v > 255) v = 255;
        return (uint8_t)v;
        };

    for (int32_t raw_idx : raw.poly_idx) {
        bool face_end = (raw_idx < 0);
        int  ctrl_idx = face_end ? (~raw_idx) : raw_idx;

        PV pv = {};

        // Position
        int base = ctrl_idx * 3;
        if (base + 2 < (int)raw.cp.size()) {
            pv.x = (float)raw.cp[base];
            pv.y = (float)raw.cp[base + 1];
            pv.z = (float)raw.cp[base + 2];
        }

        // Normal — undo exporter uniform X-negation.
        // Fix 3/4: exporter now negates only X (never Z) for all meshes.
        // Fix 6: canonicalise -0.0f → +0.0f before storing so that memcmp-based
        //   dedup in PVEq treats "zero normal" identically regardless of sign.
        //   Without this, two polygon-vertices whose normal component is exactly
        //   0.0 but arrives as -0.0f vs +0.0f would hash/compare as different
        //   entries, preventing deduplication and inflating the vert count.
        {
            double n[3] = { 0,0,0 };
            if (sample_layer(raw.normals, ctrl_idx, pv_idx, n)) {
                float rnx = -(float)n[0]; // un-negate X (exporter always negated X)
                float rny =  (float)n[1];
                float rnz =  (float)n[2]; // Z is now passed straight through (exporter no longer touches Z)
                // canonicalise -0.0f to +0.0f so memcmp dedup is stable
                pv.nx = (rnx == 0.0f) ? 0.0f : rnx;
                pv.ny = (rny == 0.0f) ? 0.0f : rny;
                pv.nz = (rnz == 0.0f) ? 0.0f : rnz;
            }
        }

        // UV0 — undo exporter Y = 1-v → v = 1-Y
        {
            double uv[2] = { 0,0 };
            if (sample_layer(raw.uv0, ctrl_idx, pv_idx, uv)) {
                pv.u = (float)uv[0];
                pv.v = 1.0f - (float)uv[1];
            }
        }

        // UV1 (second UV) — undo exporter Y = -v2 → v2 = -Y
        {
            double uv[2] = { 0,0 };
            if (sample_layer(raw.uv1, ctrl_idx, pv_idx, uv)) {
                pv.u2 = (float)uv[0];
                pv.v2 = -(float)uv[1];
            }
        }

        // Color channels (bone indices / weights stored as 0-1 floats, RGBA)
        // Channel 0: R=index1, G=index2, B=index3, A=index4
        // Channel 1: R=weight1, G=weight2, B=weight3, A=weight4
        if (raw.colors.size() >= 1) {
            double c[4] = { 0,0,0,0 };
            if (sample_layer(raw.colors[0], ctrl_idx, pv_idx, c)) {
                pv.i1 = byte_clamp(c[0]);
                pv.i2 = byte_clamp(c[1]);
                pv.i3 = byte_clamp(c[2]);
                pv.i4 = byte_clamp(c[3]);
            }
        }
        if (raw.colors.size() >= 2) {
            double c[4] = { 0,0,0,0 };
            if (sample_layer(raw.colors[1], ctrl_idx, pv_idx, c)) {
                pv.w1 = byte_clamp(c[0]);
                pv.w2 = byte_clamp(c[1]);
                pv.w3 = byte_clamp(c[2]);
                pv.w4 = byte_clamp(c[3]);
            }
        }

        pv_list.push_back(pv);
        polygon_verts.push_back((int)pv_list.size() - 1);
        ++pv_idx;

        if (face_end) {
            // FBX file must be pre-triangulated (3 vertices per face)
            assert(polygon_verts.size() == 3 && "FBX must be pre-triangulated");
            if (polygon_verts.size() == 3) {
                baked.faces.push_back({
                    (uint32_t)polygon_verts[0],
                    (uint32_t)polygon_verts[1],
                    (uint32_t)polygon_verts[2]
                    });
            }
            polygon_verts.clear();
        }
    }

    // Now build deduplicated vertex list and remap face indices
    // Use an index map keyed by raw PV data
    struct PVHash {
        size_t operator()(const PV& p) const {
            const uint8_t* b = (const uint8_t*)&p;
            size_t h = 0;
            for (size_t i = 0; i < sizeof(PV); ++i)
                h = h * 31 + b[i];
            return h;
        }
    };
    struct PVEq {
        bool operator()(const PV& a, const PV& b) const {
            return memcmp(&a, &b, sizeof(PV)) == 0;
        }
    };
    std::unordered_map<PV, uint32_t, PVHash, PVEq> cache;
    std::vector<uint32_t> remap(pv_list.size());

    for (size_t i = 0; i < pv_list.size(); ++i) {
        auto it = cache.find(pv_list[i]);
        if (it != cache.end()) {
            remap[i] = it->second;
        }
        else {
            uint32_t new_idx = (uint32_t)baked.verts.size();
            cache[pv_list[i]] = new_idx;
            remap[i] = new_idx;

            const PV& p = pv_list[i];
            FbxVertex fv = {};
            fv.x = p.x; fv.y = p.y; fv.z = p.z;
            fv.nx = p.nx; fv.ny = p.ny; fv.nz = p.nz;
            fv.u = p.u; fv.v = p.v;
            fv.u2 = p.u2; fv.v2 = p.v2;
            fv.index_1 = p.i1; fv.index_2 = p.i2;
            fv.index_3 = p.i3; fv.index_4 = p.i4;
            fv.weight_1 = p.w1; fv.weight_2 = p.w2;
            fv.weight_3 = p.w3; fv.weight_4 = p.w4;
            baked.verts.push_back(fv);
        }
    }


    // Remap face indices
    for (auto& face : baked.faces) {
        face.v1 = remap[face.v1];
        face.v2 = remap[face.v2];
        face.v3 = remap[face.v3];
    }
    DEBUG_OUTPUT(L"Mesh: %S | pv_list: %zu | verts: %zu\n",
        raw.name.c_str(), pv_list.size(), baked.verts.size());
}

// ============================================================
//  6.  Scene extraction  (Objects + Connections walk)
// ============================================================

// FbxSceneData — top-level scene container allocated on the heap by fbx_load().
// Holds all baked meshes for the file. The opaque FbxScene handle is a pointer to this.
// Freed by fbx_free() via delete.
struct FbxSceneData {
    std::vector<BakedMesh> meshes;
};

// extract_layer_elems — scan a Geometry node's children for layer element sub-nodes
// and populate the corresponding fields of rm.
// Recognised nodes: LayerElementNormal → rm.normals (3 components)
//                   LayerElementUV     → rm.uv0 then rm.uv1 (2 components each)
//                   LayerElementColor  → rm.colors[] appended (4 components, RGBA)
// Multiple UV sets are assigned in order of appearance (first=uv0, second=uv1).
static void extract_layer_elems(const FbxNode& geom_node, RawMesh& rm) {
    for (const auto& c : geom_node.children) {
        if (c.name == "LayerElementNormal") {
            fill_layer_elem(c, rm.normals, 3);
        }
        else if (c.name == "LayerElementUV") {
            // First UV goes to uv0, second to uv1
            if (rm.uv0.data.empty()) {
                fill_layer_elem(c, rm.uv0, 2);
            }
            else {
                fill_layer_elem(c, rm.uv1, 2);
            }
        }
        else if (c.name == "LayerElementColor") {
            LayerElem ce;
            fill_layer_elem(c, ce, 4);
            rm.colors.push_back(std::move(ce));
        }
    }
}

// extract_scene — walk the FBX "Objects" and "Connections" top-level nodes to build
// a list of baked meshes with their associated texture names.
//
// Pass 1 (Objects): collect every Geometry (→ RawMesh), Model (→ name string),
//   Material (→ MatInfo placeholder), and Texture (→ filename string) by their
//   unique 64-bit object IDs.
//
// Pass 2 (Connections): resolve relationships between objects:
//   Geometry → Model  : assigns the model's display name to the geometry
//   Material → Model  : links which material a model uses
//   Texture  → Material (OP "DiffuseColor" / "NormalMap" / "Bump"):
//              fills MatInfo.diffuse / MatInfo.normal
//   A second sub-pass propagates the resolved material texture names into each RawMesh.
//
// Finally, each RawMesh with valid geometry data is baked via build_mesh() and
// appended to scene.meshes.
static bool extract_scene(FbxNode& root, FbxSceneData& scene) {
    const FbxNode* objects = find_child(root, "Objects");
    const FbxNode* conns = find_child(root, "Connections");
    if (!objects) return false;

    // Collect geometries keyed by ID (map for O(1) lookup; vector preserves FBX file order)
    std::unordered_map<int64_t, RawMesh> geom_map;
    std::vector<int64_t> geom_order;   // insertion order from the Objects section
    // Collect models (node name) keyed by ID
    std::unordered_map<int64_t, std::string> model_names;
    // Model ID → column-major 4x4 local transform matrix (from Lcl Translation/Rotation/Scaling)
    std::unordered_map<int64_t, std::array<double, 16>> model_matrices;
    // Material ID → {diffuse tex name, normal tex name}
    struct MatInfo { std::string diffuse, normal; };
    std::unordered_map<int64_t, MatInfo> mat_map;
    // Texture ID → file name
    std::unordered_map<int64_t, std::string> tex_map;
    // Model ID → custom primitive_table / primitive_index properties (from export_aspose_fbx)
    std::unordered_map<int64_t, std::string> model_prim_tables;
    std::unordered_map<int64_t, int>         model_prim_indices;
    // Texture ID → usage hint (from connections: "DiffuseColor"/"NormalMap" etc.)
    // We'll resolve via Connections.

    for (const auto& obj : objects->children) {
        if (obj.props.empty()) continue;
        int64_t obj_id = 0;
        if (obj.props[0].type == 'L') obj_id = obj.props[0].s.L;
        else if (obj.props[0].type == 'I') obj_id = obj.props[0].s.I;

        if (obj.name == "Geometry") {
            RawMesh rm;
            rm.geom_id = obj_id;
            if (obj.props.size() >= 2) rm.name = obj.props[1].str;
            // Remove ":Geometry" suffix if present
            auto pos = rm.name.find(':');
            if (pos != std::string::npos) rm.name = rm.name.substr(0, pos);

            // Control points
            const FbxNode* vn = find_child(obj, "Vertices");
            if (vn && !vn->props.empty() && vn->props[0].type == 'd')
                rm.cp = vn->props[0].arr_d;
            else if (vn && !vn->props.empty() && vn->props[0].type == 'f') {
                rm.cp.resize(vn->props[0].arr_f.size());
                for (size_t i = 0; i < vn->props[0].arr_f.size(); ++i)
                    rm.cp[i] = vn->props[0].arr_f[i];
            }

            // Polygon vertex indices
            const FbxNode* pi = find_child(obj, "PolygonVertexIndex");
            if (pi && !pi->props.empty() && pi->props[0].type == 'i')
                rm.poly_idx = pi->props[0].arr_i;

            extract_layer_elems(obj, rm);
            geom_order.push_back(obj_id);   // record FBX file insertion order
            geom_map[obj_id] = std::move(rm);
        }
        else if (obj.name == "Model") {
            std::string mname;
            if (obj.props.size() >= 2) mname = obj.props[1].str;
            auto colon = mname.find(':');
            if (colon != std::string::npos) mname = mname.substr(colon + 1);
            model_names[obj_id] = mname;

            // Read Lcl Translation / Lcl Rotation / Lcl Scaling from Properties70.
            // Each P node in Properties70 has layout:
            //   props[0]=name, props[1]=type, props[2]=label, props[3]=flags,
            //   props[4]=X,    props[5]=Y,    props[6]=Z      (all 'D' scalars)
            // This matches how Blender's fbx importer reads node transforms.
            // Custom scalar properties written by Aspose Node.SetProperty() use the same
            // P node format but with only 5 entries: name, type, label, flags, value.
            double tx = 0, ty = 0, tz = 0, rx = 0, ry = 0, rz = 0, sx = 1, sy = 1, sz = 1;
            std::string model_prim_table;
            int         model_prim_index = -1;
            const FbxNode* p70 = find_child(obj, "Properties70");
            if (p70) {
                for (const auto& p : p70->children) {
                    if (p.name != "P" || p.props.empty()) continue;
                    const std::string& pname = p.props[0].str;

                    // ── custom properties (5 props: name/type/label/flags/value) ──
                    if (pname == "primitive_table" && p.props.size() >= 5) {
                        const FbxProp& val = p.props[4];
                        if (val.type == 'S') model_prim_table = val.str;
                    }
                    else if (pname == "primitive_index" && p.props.size() >= 5) {
                        const FbxProp& val = p.props[4];
                        if (val.type == 'I') model_prim_index = val.s.I;
                        else if (val.type == 'L') model_prim_index = (int)val.s.L;
                    }
                    // ── standard transform properties (7 props: name/type/label/flags/X/Y/Z) ──
                    else if (p.props.size() < 7) continue;
                    else {
                        auto get_d = [&](int idx) -> double {
                            const FbxProp& pr = p.props[idx];
                            if (pr.type == 'D') return pr.s.D;
                            if (pr.type == 'F') return pr.s.F;
                            if (pr.type == 'I') return pr.s.I;
                            return 0.0;
                            };
                        if (pname == "Lcl Translation") { tx = get_d(4); ty = get_d(5); tz = get_d(6); }
                        else if (pname == "Lcl Rotation") { rx = get_d(4); ry = get_d(5); rz = get_d(6); }
                        else if (pname == "Lcl Scaling") { sx = get_d(4); sy = get_d(5); sz = get_d(6); }
                    }
                }
            }
            // Store custom props keyed by model ID for assignment to geometry below.
            if (!model_prim_table.empty())
                model_prim_tables[obj_id] = model_prim_table;
            if (model_prim_index >= 0)
                model_prim_indices[obj_id] = model_prim_index;
            // Build column-major 4x4 matrix and store keyed by model ID.
            std::array<double, 16> mat;
            build_trs_matrix(tx, ty, tz, rx, ry, rz, sx, sy, sz, mat.data());
            model_matrices[obj_id] = mat;
        }
        else if (obj.name == "Material") {
            // Already have ID; tex assignments come from connections
            MatInfo mi;
            mat_map[obj_id] = mi;
        }
        else if (obj.name == "Texture") {
            // Look for RelativeFilename or Filename node
            const FbxNode* fn = find_child(obj, "RelativeFilename");
            if (!fn) fn = find_child(obj, "FileName");
            if (fn && !fn->props.empty())
                tex_map[obj_id] = fn->props[0].str;
        }
    }

    // Parse connections
    // Connection types: C "OO" child_id parent_id
    //                   C "OP" child_id parent_id  PropertyName
    //
    // Geometry → Model  (geom attaches to model/node)
    // Material → Model  (material assigned to model)
    // Texture  → Material  (texture assigned to material slot)
    // "DiffuseColor" / "NormalMap" / "Bump" usage from OP property name

    std::unordered_map<int64_t, int64_t> geom_to_model; // geom_id → model_id
    std::unordered_map<int64_t, int64_t> mat_to_model;  // mat_id  → model_id
    // tex_id → (mat_id, slot)
    struct TexSlot { int64_t mat_id; std::string slot; };
    std::vector<TexSlot> tex_slots;

    if (conns) {
        for (const auto& c : conns->children) {
            if (c.name != "C" || c.props.size() < 3) continue;
            std::string ctype = c.props[0].str;
            int64_t child_id = 0, parent_id = 0;
            if (c.props[1].type == 'L') child_id = c.props[1].s.L;
            else if (c.props[1].type == 'I') child_id = c.props[1].s.I;
            if (c.props[2].type == 'L') parent_id = c.props[2].s.L;
            else if (c.props[2].type == 'I') parent_id = c.props[2].s.I;

            if (geom_map.count(child_id) && model_names.count(parent_id))
                geom_to_model[child_id] = parent_id;
            else if (mat_map.count(child_id) && model_names.count(parent_id))
                mat_to_model[child_id] = parent_id;
            else if (tex_map.count(child_id) && mat_map.count(parent_id)) {
                TexSlot ts;
                ts.mat_id = parent_id;
                if (ctype == "OP" && c.props.size() >= 4)
                    ts.slot = c.props[3].str;
                tex_slots.push_back(ts);
                // Associate texture to material
                auto& mi = mat_map[parent_id];
                if (ts.slot == "DiffuseColor" || ts.slot.empty())
                    mi.diffuse = tex_map[child_id];
                else if (ts.slot == "NormalMap" || ts.slot == "Bump")
                    mi.normal = tex_map[child_id];
            }
        }

        // Second pass: assign textures to geometries via model
        // model_id → mat_id reverse map
        std::unordered_map<int64_t, int64_t> model_to_mat;
        for (auto& kv : mat_to_model)
            model_to_mat[kv.second] = kv.first;

        for (auto& kv : geom_map) {
            int64_t geom_id = kv.first;
            auto it_m = geom_to_model.find(geom_id);
            if (it_m == geom_to_model.end()) continue;
            int64_t model_id = it_m->second;

            // Name from model — model name always takes priority over geometry name.
            // The Blender object name (Model node, e.g. "~Hull~1") carries the ~ slot
            // convention that process_fbx_data() relies on.  The geometry node name
            // (Mesh data name, e.g. "Hull") must NOT override it.
            auto it_n = model_names.find(model_id);
            if (it_n != model_names.end())
                kv.second.name = it_n->second;

            // Copy local transform matrix from the model node
            auto it_mx = model_matrices.find(model_id);
            if (it_mx != model_matrices.end())
                memcpy(kv.second.matrix, it_mx->second.data(), 16 * sizeof(double));

            // Copy custom primitive_table / primitive_index from the model node
            auto it_pt = model_prim_tables.find(model_id);
            if (it_pt != model_prim_tables.end())
                kv.second.primitive_table = it_pt->second;
            auto it_pi = model_prim_indices.find(model_id);
            if (it_pi != model_prim_indices.end())
                kv.second.primitive_index = it_pi->second;

            auto it_mat = model_to_mat.find(model_id);
            if (it_mat == model_to_mat.end()) continue;
            int64_t mat_id = it_mat->second;
            auto it_mi = mat_map.find(mat_id);
            if (it_mi == mat_map.end()) continue;
            kv.second.diffuse_tex = it_mi->second.diffuse;
            kv.second.normal_tex = it_mi->second.normal;
        }
    }

    // Bake each geometry — iterate geom_order (FBX file insertion order) so that
    // scene.meshes comes out in the same sequence as the Objects section of the FBX.
    // This matches what SharpGLTF's scene.VisualChildren gives for GLB imports, and
    // ensures the slot-to-section mapping in the exporter is correct.
    //
    // Fix 1: NEVER skip empty geometries.
    // Previously: `if (rm.cp.empty() || rm.poly_idx.empty()) continue;`
    // Problem:    pure-transform nodes (locators, collision proxies, turret rings)
    //             are exported as Geometry objects with no vertices/faces.  Skipping
    //             them shifts every subsequent mesh down by one slot so that
    //             scene.meshes[i] no longer corresponds to exported item i+1.
    //             For a tank where item 4 is a dummy node, fbx_mesh_vert_count(scene,3)
    //             was returning item 5's count — the direct cause of the "4th object
    //             wrong" bug.
    // Fix:        always push a BakedMesh, even if empty.  Metadata (name, matrix,
    //             primitive_table/index) is copied unconditionally so the caller can
    //             identify the slot.  build_mesh() is called only when there is
    //             actual geometry to bake.  fbx_mesh_vert_count / fbx_mesh_face_count
    //             return 0 for empty slots, which the VB.NET caller already handles
    //             safely (Dim nativeVerts(-1) is a valid zero-length array in VB.NET).
    scene.meshes.clear();
    for (int64_t id : geom_order) {
        auto& rm = geom_map[id];
        BakedMesh bm;
        // Copy metadata unconditionally — preserves index correspondence.
        bm.name            = rm.name.empty() ? rm.name : rm.name; // assign either way
        bm.diffuse_tex     = rm.diffuse_tex;
        bm.normal_tex      = rm.normal_tex;
        bm.has_uv2         = false;
        bm.color_ch        = 0;
        memcpy(bm.matrix, rm.matrix, 16 * sizeof(double));
        bm.primitive_table = rm.primitive_table;
        bm.primitive_index = rm.primitive_index;
        // Only bake geometry when it actually exists.
        if (!rm.cp.empty() && !rm.poly_idx.empty()) {
            build_mesh(rm, bm);
            if (bm.name.empty()) bm.name = rm.name;
        }
        scene.meshes.push_back(std::move(bm));
    }

    return true;
}

// ============================================================
//  7.  File loader
// ============================================================

// read_file — load an entire file into a byte vector.
// Uses fopen_s (MSVC-safe variant) in binary mode. Returns an empty vector on failure.
static std::vector<uint8_t> read_file(const char* path) {
    FILE* f = nullptr;
    fopen_s(&f, path, "rb");
    if (!f) return {};
    fseek(f, 0, SEEK_END);
    long sz = ftell(f);
    fseek(f, 0, SEEK_SET);
    std::vector<uint8_t> buf((size_t)sz);
    fread(buf.data(), 1, (size_t)sz, f);
    fclose(f);
    return buf;
}

// ============================================================
//  8.  C API
// ============================================================

// fbx_load — load and fully parse an FBX binary file from disk.
// Reads the file, verifies the magic header, parses the node tree, extracts and bakes
// all mesh geometry. Returns an opaque FbxScene handle on success, nullptr on failure.
// The caller must eventually call fbx_free() to release the allocated FbxSceneData.
FBX_API FbxScene fbx_load(const char* path) {
    auto file_data = read_file(path);
    if (file_data.empty()) return nullptr;

    FbxNode root;
    int version = 0;
    if (!parse_fbx(file_data.data(), file_data.size(), root, version))
        return nullptr;

    FbxSceneData* sd = new FbxSceneData();
    if (!extract_scene(root, *sd)) {
        delete sd;
        return nullptr;
    }
    return (FbxScene)sd;
}

// fbx_free — release all memory allocated by fbx_load().
// Safe to call with nullptr. After this call the handle is invalid.
FBX_API void fbx_free(FbxScene scene) {
    if (scene) delete (FbxSceneData*)scene;
}

// fbx_mesh_count — return the number of baked meshes in the scene.
// Returns 0 if scene is nullptr.
FBX_API int fbx_mesh_count(FbxScene scene) {
    if (!scene) return 0;
    return (int)((FbxSceneData*)scene)->meshes.size();
}

// fbx_mesh_name — return the display name of mesh[idx] (taken from the FBX Model node).
// Returns "" on invalid scene or out-of-range idx. Pointer is valid until fbx_free().
FBX_API const char* fbx_mesh_name(FbxScene scene, int idx) {
    if (!scene) return "";
    auto& s = *(FbxSceneData*)scene;
    if (idx < 0 || idx >= (int)s.meshes.size()) return "";
    return s.meshes[idx].name.c_str();
}

// fbx_mesh_diffuse — return the diffuse texture filename for mesh[idx].
// Resolved from the Material's "DiffuseColor" texture connection in Connections.
// Returns "" if no diffuse texture was found. Pointer is valid until fbx_free().
FBX_API const char* fbx_mesh_diffuse(FbxScene scene, int idx) {
    if (!scene) return "";
    auto& s = *(FbxSceneData*)scene;
    if (idx < 0 || idx >= (int)s.meshes.size()) return "";
    return s.meshes[idx].diffuse_tex.c_str();
}

// fbx_mesh_normal_tex — return the normal-map texture filename for mesh[idx].
// Resolved from the Material's "NormalMap" or "Bump" texture connection.
// Returns "" if no normal map was found. Pointer is valid until fbx_free().
FBX_API const char* fbx_mesh_normal_tex(FbxScene scene, int idx) {
    if (!scene) return "";
    auto& s = *(FbxSceneData*)scene;
    if (idx < 0 || idx >= (int)s.meshes.size()) return "";
    return s.meshes[idx].normal_tex.c_str();
}

// fbx_mesh_vert_count — return the number of deduplicated vertices in mesh[idx].
// Allocate this many FbxVertex elements before calling fbx_mesh_get_verts().
FBX_API int fbx_mesh_vert_count(FbxScene scene, int idx) {
    if (!scene) return 0;
    auto& s = *(FbxSceneData*)scene;
    if (idx < 0 || idx >= (int)s.meshes.size()) return 0;
    return (int)s.meshes[idx].verts.size();
}

// fbx_mesh_face_count — return the number of triangles in mesh[idx].
// Allocate this many FbxFace elements before calling fbx_mesh_get_faces().
FBX_API int fbx_mesh_face_count(FbxScene scene, int idx) {
    if (!scene) return 0;
    auto& s = *(FbxSceneData*)scene;
    if (idx < 0 || idx >= (int)s.meshes.size()) return 0;
    return (int)s.meshes[idx].faces.size();
}

// fbx_mesh_has_uv2 — returns 1 if mesh[idx] has a second UV channel (uv1), 0 otherwise.
// When 1, FbxVertex.u2/v2 fields are populated; when 0 they are zero-initialised.
FBX_API int fbx_mesh_has_uv2(FbxScene scene, int idx) {
    if (!scene) return 0;
    auto& s = *(FbxSceneData*)scene;
    if (idx < 0 || idx >= (int)s.meshes.size()) return 0;
    return s.meshes[idx].has_uv2 ? 1 : 0;
}

// fbx_mesh_color_ch — return how many LayerElementColor channels mesh[idx] has.
// Aspose.3D exports bone data as two color channels:
//   channel 0 (RGBA) → FbxVertex.index_1..4  (bone indices, 0-255)
//   channel 1 (RGBA) → FbxVertex.weight_1..4 (bone weights, 0-255)
// Returns 0 if no color channels are present (mesh has no skinning data).
FBX_API int fbx_mesh_color_ch(FbxScene scene, int idx) {
    if (!scene) return 0;
    auto& s = *(FbxSceneData*)scene;
    if (idx < 0 || idx >= (int)s.meshes.size()) return 0;
    return s.meshes[idx].color_ch;
}

// fbx_mesh_get_verts — copy all vertices of mesh[idx] into the caller-supplied buffer 'out'.
// 'out' must point to at least fbx_mesh_vert_count(scene, idx) * sizeof(FbxVertex) bytes.
// Data is copied in one memcpy; no conversion is needed — all transforms are pre-applied.
FBX_API void fbx_mesh_get_verts(FbxScene scene, int idx, FbxVertex* out) {
    if (!scene || !out) return;
    auto& s = *(FbxSceneData*)scene;
    if (idx < 0 || idx >= (int)s.meshes.size()) return;
    const auto& verts = s.meshes[idx].verts;
    memcpy(out, verts.data(), verts.size() * sizeof(FbxVertex));
}

// fbx_mesh_get_faces — copy all triangle face indices of mesh[idx] into 'out'.
// 'out' must point to at least fbx_mesh_face_count(scene, idx) * sizeof(FbxFace) bytes.
// Each FbxFace holds three uint32_t indices into the vertex array returned by fbx_mesh_get_verts().
FBX_API void fbx_mesh_get_faces(FbxScene scene, int idx, FbxFace* out) {
    if (!scene || !out) return;
    auto& s = *(FbxSceneData*)scene;
    if (idx < 0 || idx >= (int)s.meshes.size()) return;
    const auto& faces = s.meshes[idx].faces;
    memcpy(out, faces.data(), faces.size() * sizeof(FbxFace));
}

// fbx_mesh_matrix — return a pointer to the 16-element column-major 4x4 local transform
// matrix for mesh[idx].  Built from the Model node's Lcl Translation, Lcl Rotation,
// and Lcl Scaling properties (Euler XYZ degrees), same as Blender's FBX importer.
// Translation is at indices [12], [13], [14].  Identity if no transform was found.
// Pointer is valid until fbx_free() is called.
FBX_API const double* fbx_mesh_matrix(FbxScene scene, int idx) {
    if (!scene) return nullptr;
    auto& s = *(FbxSceneData*)scene;
    if (idx < 0 || idx >= (int)s.meshes.size()) return nullptr;
    return s.meshes[idx].matrix;
}

// fbx_mesh_primitive_table — return the "primitive_table" custom property written by
// export_aspose_fbx via base.SetProperty("primitive_table", _group(item).table_entry_name).
// This is the exact section name from .primitives_processed (e.g. "hull/lod0/indices").
// Used by process_fbx_data() on re-import to sort meshes into the correct slots without
// relying on Blender's export order or the fragile ~ naming convention.
// Returns "" if the property was not present (older FBX files without this tag).
// Pointer is valid until fbx_free() is called.
FBX_API const char* fbx_mesh_primitive_table(FbxScene scene, int idx) {
    if (!scene) return "";
    auto& s = *(FbxSceneData*)scene;
    if (idx < 0 || idx >= (int)s.meshes.size()) return "";
    return s.meshes[idx].primitive_table.c_str();
}

// fbx_mesh_primitive_index — return the "primitive_index" custom property written by
// export_aspose_fbx via base.SetProperty("primitive_index", item).
// This is the 1-based item slot index at export time, matching the _group() array index.
// Returns -1 if the property was not present (older FBX files without this tag).
FBX_API int fbx_mesh_primitive_index(FbxScene scene, int idx) {
    if (!scene) return -1;
    auto& s = *(FbxSceneData*)scene;
    if (idx < 0 || idx >= (int)s.meshes.size()) return -1;
    return s.meshes[idx].primitive_index;
}
