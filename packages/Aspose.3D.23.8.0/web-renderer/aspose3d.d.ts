
declare namespace Aspose3D {
    export type Vector2 = [number, number];
    export type Vector3 = [number, number, number];
    export type Matrix4 = [number, number, number, number,number, number, number, number,number, number, number, number,number, number, number, number];

    export type BoundingBox = {
        minimum : Vector3;
        maximum : Vector3;
        isNull() : boolean;
    }

    export enum Axis { X , Y , Z }
    export enum Unit {
        Millimeters,
        Centimeters,
        Inches,
        Feet,
        Yards,
        Meters,
        Kilometers,
        Miles
    }
    export enum BlendFactor {
        Zero = 0,
        One = 1,
        SrcColor = 0x0300,//Passed to blendFunc or blendFuncSeparate to multiply a component by the source elements color.
        OneMinusSrcColor = 0x0301, //	Passed to blendFunc or blendFuncSeparate to multiply a component by one minus the source elements color.
        SrcAlpha = 0x0302, //	Passed to blendFunc or blendFuncSeparate to multiply a component by the source's alpha.
        OneMinusSrcALPHA = 0x0303, //	Passed to blendFunc or blendFuncSeparate to multiply a component by one minus the source's alpha.
        DstAlpha = 0x0304, //	Passed to blendFunc or blendFuncSeparate to multiply a component by the destination's alpha.
        OneMinusDstAlpha = 0x0305, //	Passed to blendFunc or blendFuncSeparate to multiply a component by one minus the destination's alpha.
        DstColor = 0x0306, //	Passed to blendFunc or blendFuncSeparate to multiply a component by the destination's color.
        OneMinusDstColor = 0x0307, //	Passed to blendFunc or blendFuncSeparate to multiply a component by one minus the destination's color.
        SrcAlphaSaturate = 0x0308, //	Passed to blendFunc or blendFuncSeparate to multiply a component by the minimum of source's alpha or one minus the destination's alpha.
        ConstantColor = 0x8001, //	Passed to blendFunc or blendFuncSeparate to specify a constant color blend function.
        OneMinusConstantColor = 0x8002, //	Passed to blendFunc or blendFuncSeparate to specify one minus a constant color blend function.
        ConstantAlpha = 0x8003, //	Passed to blendFunc or blendFuncSeparate to specify a constant alpha blend function.
        OneMinusConstantAlpha = 0x8004, //	Passed to blendFunc or blendFuncSeparate to specify one minus a constant alpha blend function.
    }
    export enum CompareFunction {
        Always,
        Never,
        Equal,
        NotEqual,
        Greater,
        GEqual,
        Less,
        LEqual
    }
    export class A3DObject {
        /**
         * Gets the unique id of current object
         */
        public get objectId() : number;
        /**
         * Gets the object's name
         */
        public get name() : string;
        /**
         * Sets the object's name
         */
        public set name(v : string);
        /**
         * Gets the object's type name
         */
        public get typeName() : string;
    }

    /**
     * The meta data of the scene
     */
    export class SceneMetaData {
        private constructor();
        public get fileName() : string;
        public get fileSize() : number;
        public get comment() : string;
        public get creationTime() : string;
        public get author() : string;
        public get authoringTool() : string;
        public get format() : string;
        public get keywords() : string;
        public get revision() : string;
        public get subject() : string;
        public get title() : string;
        public get unitName() : string;
        public get unitScaleFactor() : number;
        public get originalUp() : Axis;
        public get up() : Axis;
        public get preferredUnit() : Unit;
    }
    /**
     * Scene instance
     */
    export class Scene {
        public constructor();
        /**
         * Get the root node of the scene
         */
        public get rootNode() : Node;
        public createNode() : Node;
        public getMetaData() : SceneMetaData;
        /**
         * Load scene from array buffer 
         * @param buffer  
         */
        public static fromBuffer(renderer : Renderer, buffer : ArrayBuffer): Scene;

        /**
         * Query object by it's object id, including Node/entity
         * @param objectId Object's id to query 
         */
        public queryObject(objectId : number) : Node | Entity | null;
        /**
         * Encode current scene into USDZ format and return it in callback.
         * @param callback 
         */
        public toBuffer() : Promise<Uint8Array>;
    }
    export enum TransformOp
    {
        Transform,
        Translate,
        Scale,
        RotateByX,
        RotateByY,
        RotateByZ,
        RotateByXYZ,
        RotateByAxis
    }
    export class TransformOperator
    {
        public get vec() : Vector3;
        public set vec(v : Vector3);
        public get angle() : number;
        public set angle(v : number);
        public get name() : string;
        public set name(val : string);
        public get operation() : TransformOp;
    }
    export class Transform {
        private constructor();
        public getMatrix() : Matrix4;
        public clear();
        public translate(x : number, y : number, z : number);
        public scale(x : number, y : number, z : number);
        public get numOperators() : number;
        public getOperator(idx : number) : TransformOperator;
        public prepareOperator(op : TransformOp) : TransformOperator;
        public findOperator(op : TransformOp) : TransformOperator | null;
        public insert(idx : number, op : TransformOp) : boolean;
        public rotateByX(angle : number);
        public rotateByY(angle : number);
        public rotateByZ(angle : number);
        public rotateByXYZ(x : number, y : number, z : number);
        public append(matrix : Matrix4);
        public rotateBy(angle : number, axis : Vector3);
    }
    export enum NodeFlag
    {
        /// <summary>
        /// The entities can be rendered when it's inside the frustum, default is true 
        /// </summary>
        Visible,
        /// <summary>
        /// The node has a bounding box, default is true
        /// </summary>
        HasBoundingBox,
        /// <summary>
        /// The node is visible in the BIM inspector, so properties shall be viewed to user.
        /// </summary>
        BIMInspectorVisible,
        /// <summary>
        /// Make the node visible in property grid
        /// </summary>
        PropertyGridVisible,
        /// <summary>
        /// Locked node should not be moved
        /// </summary>
        Locked,
        // The node will not be exported
        Excluded,
    }
    /**
     * Scene node used to construct scene hierarchy structure
     */
    export class Node extends A3DObject {
        private constructor();

        public addChildNode(child : Node) : void;
        public addEntity(entity : Entity) : void;
        public addMaterial(material : Material) : void;
        public clearMaterials() : void;

        public clearEntities() : void;
        public createChildNode(entity : Entity) : Node;
        public getChildNode(childName : string) : Node;
        public removeEntity(entity : Entity) : void;
        public removeNode(child : Node) : void;

        public hasFlag(flag : NodeFlag) : boolean;
        public setFlag(flag : NodeFlag, value : boolean);
        /**
         * Gets the default entity of current node
         */
        public get entity() : Entity;
        /**
         * Sets the default entity of current node
         */
        public set entity(v : Entity);

        /**
         * Gets the node's position in global system.
         */
        public get globalPosition() : Vector3;
        /**
         * Gets the transformation in world coordinate system.
         */
        public worldTransform() : Matrix4;
        /**
         * Gets the transformation in local coordinate system.
         */
        public localTransform() : Transform;
        /**
         * Gets the transformation in geometric coordinate system.
         * This is similar to localTransform but only applied to geometries attached to current node.
         */
        public geometricTransform() : Transform;
        /**
         * Gets the bounding box of current node
         */
        public getBoundingBox() : BoundingBox;
        /**
         * Gets the parent node of current node
         */
        public get parent() : Node | null;
        /**
         * Gets the scene instance that this node belongs to
         */
        public get scene() : Scene;
        /**
         * Gets the children nodes
         */
        public get children() : NodeCollection;
        /**
         * Gets the entities
         */
        public get entities() : EntityCollection;
        /**
         * Gets the materials
         */
        public get materials() : MaterialCollection;
        /**
         * Gets the first material
         */
        public get material() : Material;
        /**
         * Sets the first material
         */
        public set material(v : Material);
    }
    interface EmscriptenCollection<T> {
        size() : number;
        get(idx : number) : T;
        set(idx : number, value : T);
        push_back(value : T);
    }

    type NodeCollection = EmscriptenCollection<Node>;
    type EntityCollection = EmscriptenCollection<Entity>;
    type MaterialCollection = EmscriptenCollection<Material>;

    type RenderStateDesc = {
        cullFace? : boolean;
        depthTest? : boolean;
        depthWrite? : boolean;
        depthFunction? : CompareFunction;
        blend? : boolean;
        sourceBlendFactor? : BlendFactor;
        destinationBlendFactor? : BlendFactor;
        polygonOffset? : boolean;
        polygonOffsetUnit? : number;
        polygonOffsetFactor? : number;
    }
    /**
     * Base class of all materials
     */
    export class Material extends A3DObject {
        protected constructor();
        public setRenderState(desc : RenderStateDesc);
        public getTexture(slot : string) : TextureUnit | null;
        public setTexture(slot : string, tex : TextureUnit | null);
    }
    /**
     * Blinn-phong shading model
     */
    export class PhongMaterial extends Material {
        protected constructor();
        public get diffuse() : Vector3;
        public set diffuse(v : Vector3);

        public get ambient() : Vector3;
        public set ambient(v : Vector3);

        public get specular() : Vector3;
        public set specular(v : Vector3);

        public get shininess() : number;
        public set shininess(v : number);

        public get specularFactor() : number;
        public set specularFactor(v : number);

        public get transparentColor() : Vector3;
        public set transparentColor(v : Vector3);

        public get transparency() : number;
        public set transparency(v : number);

        public get emissive() : Vector3;
        public set emissive(v : Vector3);

        public get lighting() : Boolean;
        public set lighting(v : Boolean);

        public setTexture(slot : 'diffuse' | 'normal' | 'emissive' | 'specular', tex : TextureUnit);
    }
    /**
     * Physical-based rendering model.
     */
    export class PbrMaterial extends Material {
        protected constructor();
        public get albedo() : Vector3;
        public set albedo(v : Vector3);

        public get ambient() : Vector3;
        public set ambient(v : Vector3);

        public get emissive() : Vector3;
        public set emissive(v : Vector3);

        public get metallic() : number;
        public set metallic(v : number);

        public get roughness() : number;
        public set roughness(v : number);

        public get occlusion() : number;
        public set occlusion(v : number);

        public get transparency() : number;
        public set transparency(v : number);

        public get lighting() : number;
        public set lighting(v : number);

        public setTexture(slot : 'albedo' | 'normal' | 'emissive' | 'specular' | 'occlusion' | 'metallicRoughness', tex : TextureUnit);
    }
    export class TextureUnit {
        protected constructor();
        /**
         * Gets the width of the texture unit
         */
        public get width() : number;
        /**
         * Gets the height of the texture unit
         */
        public get height() : number;
        /**
         * Gets the file name of the texture unit
         */
        public get fileName() : string;
        /**
         * Sets the file name of the texture unit
         */
        public set fileName(v : string);
        
        /**
         * Sets the raw content of the texture unit(in both GPU and CPU)
         * @param data 
         */
        public setContent(data : Uint8Array);
        /**
         * Gets the raw content of the texture unit in CPU
         */
        public getContent() : Promise<Uint8Array | null>;
        /**
         * Gets the raw content of the texture unit in CPU
         */
        public getContentImpl(cb : (data : Uint8Array | null) => void);
    }
    export class RenderWindow {
        /**
         * Construct a render window from the canva
         * @param canvasId 
         */
        public constructor(canvasId : String);

        /**
         * Start the render loop, then the canvas will automatically update its content without manually render 
         */
        public startRenderLoop();

        public makeCurrent();

        public project(pos : Vector3) : Vector2;
    }
    /**
     * The base class of all entities that can be attached to the node.
     */
    export class Entity extends A3DObject {
        protected constructor();
        public get scene() : Scene;        
        public get parent() : Node;        
        public get __debug() : boolean;
        public set __debug(v : boolean);

        public get visible() : boolean;
        public set visible(v : boolean);

        public get receiveShadows() : boolean;
        public set receiveShadows(v : boolean);

        public get castShadows() : boolean;
        public set castShadows(v : boolean);
    }
    export enum ProjectionType 
    {
        Ortho,
        Perspective
    }
    export class Frustum extends Entity {
        public get up() : Vector3;
        public set up(v : Vector3);
        public get direction() : Vector3;
        public set direction(v : Vector3);
        public get fov() : number;
        public set fov(v : number);
        public get near() : number;
        public set near(v : number);
        public get far() : number;
        public set far(v : number);
        public lookAt(target : Vector3) : void;
    }
    export class Camera extends Frustum {
        public constructor();
        public get projectionType() : ProjectionType;
        public set projectionType(v : ProjectionType);
    }
    /**
     * Light 
     */
    export class Light extends Frustum {
        public constructor();
        /**
         * Gets whether the light is enabled
         */
        public get enabled() : boolean;
        /**
         * Sets whether the light is enabled
         */
        public set enabled(v : boolean);
        /**
         * Gets the light type
         */
        public get type() : LightType;
        /**
         * Sets the light type
         */
        public set type(type : LightType);
        /**
         * Gets the color of the light
         */
        public get color() : Vector3;
        /**
         * Sets the color of the light
         */
        public set color(v : Vector3);


        public get falloff() : number;
        public set falloff(v : number);
        public get hotSpot() : number;
        public set hotSpot(v : number);
        public get constantAttenuation() : number;
        public set constantAttenuation(v : number);
        public get linearAttenuation() : number;
        public set linearAttenuation(v : number);
        public get quadraticAttenuation() : number;
        public set quadraticAttenuation(v : number);
        public get intensity() : number;
        public set intensity(v : number);

    }
    /**
     * Light's type
     */
    export enum LightType
    {
        /**
         * The light is a directional light, it's position will not affect the shading.
         */
        Directional,
        /**
         * The light is a omni light source, it's direction will not affect the shading.
         */
        Omni,
        /**
         * The light is a spot light, it's position and direction will affect the shading.
         */
        Spot
    }
    export class Mesh extends Entity {

    }
    export class AMesh extends Entity {

    }

    type VertexAttribute = {
        attr : 'position' | 'normal' | 'uv';
        data : ArrayBufferLike
    }
    /**
     * The topology used in BufferedGeometry
     */
    export enum PrimitiveTopology
    {
        /**
         * The geometry contains the list of points
         */
        PointList,
        /**
         * The geometry contains the list of lines.
         * A line list with N lines requires N * 2 elements(indices/vertices).
         */
        LineList,
        /**
         * The geometry contains the strip of lines.
         * A line strip with N lines requires N + 1 elements(indices/vertices).
         */
        LineStrip,
        /**
         * The geometry contains the list of triangles.
         * A triangle list with N triangles requires N * 3 elements.
         */
        TriangleList,
        /**
         * The geometry contains the strip of triangles.
         * A triangle strip with N triangles requires N + 2 elements(indices/vertices).
         */
        TriangleStrip,
        /**
         * The geometry contains the fan of triangles.
         * A triangle fan with N triangles requires N + 2 elements.
         */
        TriangleFan,
    }
    /**
     * BufferedGeometry contains the definition of a customized low-level renderable object.
     */
    export class BufferedGeometry extends Entity {
        public constructor();

        /**
         * Gets the topology of BufferedGeometry, default value is @see PrimitiveTopology.TriangleList
         */
        public get topology() : PrimitiveTopology;
        /**
         * Sets the topology of BufferedGeometry, default value is @see PrimitiveTopology.TriangleList
         */
        public set topology(v : PrimitiveTopology);

        /**
         * Loads the vertices data of the geometry.
         * @param attributes 
         */
        public loadVertices(attributes : VertexAttribute[]);
        /**
         * Loads the indices data of the geometry, indices are optional to render.
         * @param indices 
         */
        public loadIndices(indices : Uint32Array | Uint16Array | Uint8Array);

    }
    /**
     * This entity encapsulates a set of lines
     */
    export class LineSet extends Entity {
        public constructor();
        /**
         * Clear all lines
         */
        public clear();
        /**
         * Append a new line to this line set instance. 
         * @param from 
         * @param to 
         */
        public line(from : Vector3, to : Vector3);
        /**
         * Clear all lines and create lines based on a set of continous control points 
         * @param points 
         */
        public lineStrip(points : Vector3[]);
    }

    /**
     * Manages the command to extend the renderer
     */
    export class CommandManager
    {
        protected constructor();
        /**
         * Register a top-level command
         * @param cmd The command to register
         */
        public registerCommand(cmd : Command);
        /**
         * Register a sub command under specified parent command 
         * @param parent The name of the parent command
         * @param cmd The command to register
         */
        public registerCommandEx(parent : string, cmd : Command);

        /**
         * Execute command in specified path
         * @param path 
         */
        public invoke(path : string) : boolean;

    }
    export type CommandDef = {
        /**
         * Name of the command
         */
        name : string;
        /**
         * Display name of the command for human reading.
         */
        text : string;
        /**
         * Implementation of this command when it is get called.
         */
        execute : () => void;
        /**
         * Called when the command was registered to the command manager.
         */
        registered? : () => void;
        /**
         * Return the checked status of this command.
         */
        checked? : () => boolean;
    }
    /**
     * The command implementation
     */
    export class Command {
        public constructor(def : CommandDef);
        /**
         * Manually execute this command
         */
        public execute() : void;
        /**
         * Return true if this command is checked.
         */
        public checked() : boolean;

        /**
         * Gets the name of this command
         */
        public get name() : string;
        /**
         * Gets the text of this command that may used in the menu entry.
         */
        public get text() : string;
        /**
         * Sets the text of this command that may used in the menu entry.
         */
        public set text(v : string);

    }

    interface RendererEventMap {
        "keydown" : KeyboardEvent;
        "keyup" : KeyboardEvent;
        "keypress" : KeyboardEvent;
        "mousedown" : MouseEvent;
        "mouseup" : MouseEvent;
        "mousemove" : MouseEvent;
        "wheel" : WheelEvent;
        "selectionChanged" : Event;
        "sceneChanged" : Event;
    }
    export type EventType = keyof RendererEventMap;

    export type RendererFeature =
        //Enable the main menu
        | "menu"
        //Highlight the selected object
        | "selection"
        //Enable the BIM viewer
        | "bim"
        //Enable the property grid
        | "property-grid"
        //Enable the reference grid
        | "grid"
        //Enable the summary information panel
        | "summary"
        ;


    export enum ShadingModel
    {
        /**
         * Blinn-phong shading model
         */
        Phong,
        /**
         * Lambert shading model
         */
        Lambert,
        /**
         * The physical-based rendering material.
         */
        Pbr
    }

    /**
     * Internal variant
     */
    export class Variant<T> {
        /**
         * Gets the name of the variant
         */
        public get name() : string;
        /**
         * Gets the value of this variant
         */
        public getValue() : T;
        /**
         * Sets the value of the variant
         * @param val 
         */
        public setValue(val : T);
    }
    export class VariantManager {
        public getVariant<T>(variantName : string) : null | Variant<T>;
        public subscribe(variantName : string, callback : (v : Variant<any>) => void);
    }
    export class Shadow {
        public constructor(renderer : Renderer, width : number, height : number);
        /**
         * Gets which light will enable shadow
         */
        public get light() : Light
        /**
         * Sets which light will enable shadow
         */
        public set light(v : Light);
    }
    export enum LightingScheme {
        NoLights,
        Default
    }
    export abstract class Selection {
        /**
         * Clear all selection
         */
		public clear() : void;
        public get length() : number;
    }
    export class NodesSelection extends Selection {
        public constructor();
        /**
         * Add new node to selection
         * @param node Node to be selected
         */
		public add(node : Node) : void;
        /**
         * Remove node from selections
         * @param node Node to be unselected
         */
		public remove(node : Node) : void;
        /**
         * Check if node is inside the selection collection
         * @param node Node to be checked if selected
         */
		public has(node : Node) : boolean;

        public get(idx : number) : Node | null;
    }
    /**
     * Mesh selection mode
     */
    export enum SelectionMode {
        /**
         * Vertex selection mode
         */
        Vertex,
        /**
         * Edge selection mode
         */
        Edge,
        /**
         * Polygon selection mode
         */
        Polygon
    }
    export class MeshSelection extends Selection {
        /**
         * 
         * @param mesh Which mesh will be used to select
         * @param mode Mesh selection mode
         */
        public constructor(mesh : AMesh, mode : SelectionMode);
        /**
         * Add mesh component to selection collection
         */
		public add(node : number) : void;
        /**
         * Remove mesh component from selection collection
         */
		public remove(node : number) : void;
        /**
         * Check if selected part is inside selection collection
         */
		public has(node : number) : boolean;
        /**
         * Gets selection mode
         */
        public get mode() : SelectionMode;
        /**
         * Sets selection mode
         */
        public set mode(v : SelectionMode);

        public get(idx : number) : Node | null;
    }
    export class Renderer {
        /**
         * Construct a new renderer based on specified render window
         * @param renderWindow Which render window will be rendered to.
         */
        public constructor(renderWindow : RenderWindow);
        /**
         * Create a new material instance.
         * @param shadingModel Material's shading model
         */
        public createMaterial(shadingModel : ShadingModel) : Material;
        /**
         * Create a new texture unit instance.
         */
        public createTextureUnit() : TextureUnit;

        /**
         * Gets current shadow
         */
        public get shadow() : Shadow;
        /**
         * Sets current shadow
         */
        public set shadow(v : Shadow);

        /**
         * Gets the current lighting scheme
         */
        public get lightingScheme() : LightingScheme;
        /**
         * Sets the lighting scheme, default value is Default
         */
        public set lightingScheme(v : LightingScheme);

        /**
         * Gets the variant manager
         */
        public get variantManager() : VariantManager;

        /**
         * Gets current camera
         */
        public get camera() : Camera;
        /**
         * Sets current camera
         */
        public set camera(v : Camera);
        /**
         * Gets current scene
         */
        public get scene() : Scene;
        /**
         * Sets current scene
         */
        public set scene(v : Scene);

        /**
         * Gets current movement
         */
        public get movement() : Movement?;

        /**
         * Sets current movement
         */
        public set movement(v : Movement?);

        /**
         * Gets current render window
         */
        public get window() : RenderWindow;

        /**
         * Gets the time elapsed since the renderer was created.
         */
        public time() : number;

        /**
         * Invalidates the render window to request for a repaint.
         */
        public invalidate() : void;

        /**
         * Pick the 3D world position from given 2D screen position
         * @param x The x component of the screen position ranged in [0, 1]
         * @param y The y component of the screen position ranged in [0, 1]
         */
        public pick(x : number, y : number) : Vector3 | null;

        /**
         * Take a snapshot from current frame buffer and get the encoded image data.
         * @return Returns the size and encoded image data
         */
        public snapshot() : Promise<{width : number, height : number, buffer : Uint8Array}>;

        /**
         * Register the widget controller to this renderer.
         * @param controller The widget controller to register
         */
        public registerUI(controller : WidgetController);

        /**
         * Add a event listener to specified renderer's internal event
         * @param event The event to register
         * @param listener The event handler
         */
        public addEventListener<K extends keyof RendererEventMap>(event : K, listener : (this: Renderer, e : RendererEventMap[K]) => void);

        /**
         * Enable a renderer feature
         * @param feature The feature to enable
         */
        public enableFeature(feature : RendererFeature); 

        /**
         * Gets the command manager
         */
        public getCommandManager() : CommandManager;
        /**
         * Dispatch a event to renderer's internal component
         */
        public dispatchEvent(e : object) : void


        /**
         * Gets current selection object
         */
        public get selection() : Selection;

        /**
         * Sets current selection object
         */
        public set selection(val : Selection);
    }


    /**
     * The movement defines how camera will be interacted with user
     */
    export class Movement
    {
        public constructor(v : 'orbital' | 'blender' | 'maya' | 'max' | 'fps');

    }
    /**
     * The camera moves in a sphere orbital.
     */
    export class OrbitalMovement extends Movement
    {
        public constructor();
        /**
         * Gets the center of the orbit
         */
        public get center() : Vector3;
        /**
         * Sets the center of the orbit
         */
        public set center(v : Vector3);

        /**
         * The maximum distance to the center
         */
        public get maximumDistance() : number;
        /**
         * The maximum distance to the center
         */
        public set maximumDistance(v : number);

        /**
         * The minimum distance to the center
         */
        public get minimumDistance() : number;
        /**
         * The minimum distance to the center
         */
        public set minimumDistance(v : number);
    }

    /**
     * The base class of all kinds of curves
     */
    export class Curve 
    {
        protected constructor();
    }
    export class Line extends Curve
    {
        public constructor();
        /**
         * Construct a line curve from two points
         * @param from 
         * @param to 
         */
        public static fromPoints(from : Vector3, to : Vector3) : Line;

        /**
         * Gets the first point of the line
         */
        public get p0() : Vector3;
        /**
         * Sets the first point of the line
         */
        public set p0(v : Vector3);
        /**
         * Gets the second point of the line
         */
        public get p1() : Vector3;
        /**
         * Sets the second point of the line
         */
        public set p1(v : Vector3);
    }

    export class Ellipse extends Curve
    {
        public constructor()
        public get semiAxis1() : number;
        public set semiAxis1(v : number);
        public get semiAxis2() : number;
        public set semiAxis2(v : number);
    }
    export class Circle extends Curve
    {
        public constructor();
        public static create(radius : number) : Circle;
        public get radius() : number;
        public set radius(v : number);
    }
    /**
     * This class encapsulates the end point of a curve without knowing concrete representation. 
     */
    export class EndPoint
    {
        protected constructor();
        public setDegree(v : number);
        public setRadian(v : number);
        public setPoint(v : Vector3);
    }
    /**
     * Trimmed curve with specified start and final end point. 
     */
    export class TrimmedCurve extends Curve
    {
        public constructor();
        public get basisCurve() : Curve;
        public set basisCurve(v : Curve);
        public getFirst() : EndPoint;
        public getSecond() : EndPoint;
    }
    export class TransformedCurve extends Curve
    {
        public constructor();
        public static create(curve : Curve, transform : Matrix4) : TransformedCurve;
        public get basisCurve() : Curve;
        public set basisCurve(v : Curve);

        public get transformMatrix() : Matrix4;
        public set transformMatrix(v : Matrix4);
    }
    export class CompositeCurve extends Curve
    {
        public constructor();
        public addSegment(curve : Curve, sameDirection : boolean);
    }


    export type ModelEntryDef = {
        type : 'int' | 'float';
        value : number
    } | {
        type : 'string';
        value : string
    } | {
        type : 'bool';
        value : boolean;
    } | {
        type : 'vec2';
        value : Vector2;
    } | {
        type : 'vec3';
        value : Vector3;
    };

    export type ModelDef = {[T : string] : ModelEntryDef};

    export type WidgetDef = {
        type : 'ComboBox';
        id? : string;
        items : Array<string>
    } | {
        type : 'Button',
        id? : string,
        text? : string,
    } | {
        type : 'Text';
        id? : string;
        text? : string;
    } | {
        type : 'Window';
        pos? : Vector2;
        size? : Vector2;
        id? : string;
        text? : string;
        children? : Array<WidgetDef>;
    }

    export type ActionDef = {
        [id : string] : (controller : WidgetController) => void
    }

    export class WidgetController {
        public constructor(model : ModelDef, widget : WidgetDef, actions : ActionDef)
        public get(modelEntry : string) : any;
        public set(modelEntry : string, value : any);
        /**
         * Sets the property of the widget by it's id 
         * @param widgetId The widget's id to set
         * @param prop The widget's property name to set
         * @param value The value to assign to the widget's property
         */
        public setWidgetProperty(widgetId : string, prop : string, value : any);
    }

    export class Primitive extends Entity {

        /**
         * Convert current primitive to mesh
         */
        public toMesh() : AMesh;
    }

    export class Box extends Primitive {
        public constructor();
        /**
         * Gets the length of the box
         */
        public get length() : number;
        /**
         * Sets the length of the box
         */
        public set length(v : number);
        /**
         * Gets the width of the box
         */
        public get width() : number;
        /**
         * Sets the width of the box
         */
        public set width(v : number);
        /**
         * Gets the height of the box
         */
        public get height() : number;
        /**
         * Sets the height of the box
         */
        public set height(v : number);
        /**
         * Gets the number of segments in length direction
         */
        public get lengthSegments() : number;
        /**
         * Sets the number of segments in length direction
         */
        public set lengthSegments(v : number);
        /**
         * Gets the number of segments in width direction
         */
        public get widthSegments() : number;
        /**
         * Sets the number of segments in width direction
         */
        public set widthSegments(v : number);
        /**
         * Gets the number of segments in height direction
         */
        public get heightSegments() : number;
        /**
         * Sets the number of segments in height direction
         */
        public set heightSegments(v : number);

    }


    export class Cylinder extends Primitive {
        public constructor();
        /**
         * Gets the radius of top cap
         */
        public get radiusTop() : number;
        /**
         * Sets the radius of top cap
         */
        public set radiusTop(v : number);
        /**
         * Gets the radius of bottom cap
         */
        public get radiusBottom() : number;
        /**
         * Sets the radius of bottom cap
         */
        public set radiusBottom(v : number);
        /**
         * Gets the height of the cylinder
         */
        public get height() : number;
        /**
         * Sets the height of the cylinder
         */
        public set height(v : number);
        /**
         * Gets the radial segments
         */
        public get radialSegments() : number;
        public set radialSegments(v : number);
        public get heightSegments() : number;
        public set heightSegments(v : number);
        /**
         * Gets whether there's a cap in the top and bottom side of the cylinder
         */
        public get openEnded() : boolean;
        /**
         * Sets whether there's a cap in the top and bottom side of the cylinder
         */
        public set openEnded(v : boolean);
        
        /**
         * Gets the theta start
         */
        public get thetaStart() : number;
        public set thetaStart(v : number);
        public get thetaLength() : number;
        public set thetaLength(v : number);

        /**
         * Offset of the top cap
         */
        public get offsetTop() : Vector3;
        /**
         * Offset of the top cap
         */
        public set offsetTop(v : Vector3);
        /**
         * Offsets of the bottom cap
         */
        public get offsetBottom() : Vector3;
        public set offsetBottom(v : Vector3);
        /**
         * Shear of the bottom cap
         */
        public get shearBottom() : Vector2;
        public set shearBottom(v : Vector2);
        
        /**
         * Shear of the top cap
         */
        public get shearTop() : Vector2;
        public set shearTop(v : Vector2);
        
        /**
         * Gets whether to generate a fan cylinder
         */
        public get generateFanCylinder() : boolean;
        /**
         * Sets whether to generate a fan cylinder
         */
        public set generateFanCylinder(v : boolean);
    }
    export class Dish extends Primitive {
        public constructor();
        /**
         * The segments in width direction
         */
        public get widthSegments() : number;
        public set widthSegments(v : number);
        /**
         * The segments in height direction
         */
        public get heightSegments() : number;
        public set heightSegments(v : number);
        /**
         * Height of the dish
         */
        public get height() : number;
        public set height(v : number);
        /**
         * Radius of the dish
         */
        public get radius() : number;
        public set radius(v : number);
    }
    export class Plane extends Primitive {
        public constructor();
        /**
         * Length of the plane
         */
        public get length() : number;
        public set length(v : number);
        /**
         * Width of the plane
         */
        public get width() : number;
        public set width(v : number);
        /**
         * Segments in length direction
         */
        public get lengthSegments() : number;
        public set lengthSegments(v : number);
        /**
         * Segments in width segments
         */
        public get widthSegments() : number;
        public set widthSegments(v : number);
        /**
         * Up vector of the plane
         */
        public get up() : Vector3;
        public set up(v : Vector3);
    }

    export class Pyramid extends Primitive {
        public constructor();
        /**
         * The size of the pyramid's bottom size
         */
        public get bottomArea() : Vector2;
        public set bottomArea(v : Vector2);
        /**
         * The size of the pyramid's top size
         */
        public get topArea() : Vector2;
        public set topArea(v : Vector2);
        /**
         * The offset of the pyramid's bottom plane
         */
        public get bottomOffset() : number;
        public set bottomOffset(v : number);
        /**
         * Height of the pyramid 
         */
        public get height() : number;
        public set height(v : number);
    }

    export class RectangularTorus extends Primitive {
        public constructor();
        /**
         * Inner radius of the rectangular torus
         */
        public get innerRadius() : number;
        public set innerRadius(v : number);
        /**
         * Outer radius of the rectangular torus
         */
        public get outerRadius() : number;
        public set outerRadius(v : number);
        /**
         * Height of the torus
         */
        public get height() : number;
        public set height(v : number);
        /**
         * Arc of the torus, measured in radian.
         */
        public get arc() : number;
        public set arc(v : number);
        /**
         * Angle to start the arc, measured in radian.
         */
        public get angleStart() : number;
        public set angleStart(v : number);
        /**
         * Radial segments of the torus
         */
        public get radialSegments() : number;
        public set radialSegments(v : number);
    }

    export class Sphere extends Primitive {
        public constructor();
        /**
         * Radius of the sphere
         */
        public get radius() : number;
        public set radius(v : number);
        /**
         * Segments in width direction
         */
        public get widthSegments() : number;
        public set widthSegments(v : number);
        /**
         * Segments in height direction
         */
        public get heightSegments() : number;
        public set heightSegments(v : number);
        /**
         * Phi start
         */
        public get phiStart() : number;
        public set phiStart(v : number);
        /**
         * Phi length
         */
        public get phiLength() : number;
        public set phiLength(v : number);
        /**
         * Theta start.
         */
        public get thetaStart() : number;
        public set thetaStart(v : number);
        /**
         * Theta length.
         */
        public get thetaLength() : number;
        public set thetaLength(v : number);
    }

    export class Torus extends Primitive {
        public constructor();
        /**
         * Radius of the torus
         */
        public get radius() : number;
        public set radius(v : number);
        /**
         * Tube radius of the torus
         */
        public get tube() : number;
        public set tube(v : number);
        /**
         * Radial segments of the torus.
         */
        public get radialSegments() : number;
        public set radialSegments(v : number);
        /**
         * Segments per tubular
         */
        public get tubularSegments() : number;
        public set tubularSegments(v : number);
        /**
         * Arc of the torus
         */
        public get arc() : number;
        public set arc(v : number);
    }


    type Keyframe = [time : number, value : Vector3];
    type KeyframeSequence = Keyframe[];
    export class BindPoint {
        private constructor();
        /**
         * Create a bind point to animate node's translate operator in the given keyframe sequence. 
         * @param node 
         * @param operatorIdx 
         * @param keyframes 
         */
        static translation(node : Node, operatorIdx : number, keyframes : KeyframeSequence) : BindPoint;
        static scaling(node : Node, operatorIdx : number, keyframes : KeyframeSequence) : BindPoint;
    }
    export class AnimationClip {
        public get loop() : boolean;
        public set loop(v : boolean);

        /**
         * Gets whether to dispose this clip when it finished playing
         */
        public get autoDispose() : boolean;
        /**
         * Sets whether to dispose this clip when it finished playing 
         */
        public set autoDispose(v : boolean);

        /**
         * Add a bind point instance to this clip
         * @param bindPoint 
         */
        public addBindPoint(bindPoint : BindPoint) : void;

        /**
         * Play current clip
         * @param renderer 
         */
        public play(renderer : Renderer) : void;
    }

    type ParticleSystemEffector = {
        type : "gravity";
        gravity? : Vector3;
    } | {
        type : "age";
        life? : number;
    };
    type ParticleSystemEmitter = {
        //Point emitter
        type : "point";
        //Number of batches to emit per second
        batchPerSeconds? : number;
        //Number of particles per batch
        particlesPerBatch? : number;
        //size of the particle
        size? : number;
        //Offset of the particle to the particle system
        offset? : Vector3;
        //Initial velocity of the particle
        velocity? : Vector3;
    };
    type ParticleSystemAppearance = {
        //Texture of the particle
        texture : string;
        //Rows of the UV animation grid in texture, default value is 1
        rows? : number;
        //Columns of the UV animation grid in texture, default value is 1
        columns? : number;
        //Life of the particle, used in animation calculation, default value is 1.0 second
        lifeTime? : number;
    };
    type ParticleTemplateDefinition = {
        maximumParticles? : number;
        appearance : ParticleSystemAppearance;
        emitter : ParticleSystemEmitter;
        effectors? : ParticleSystemEffector[];
    }
    export class ParticleTemplate {
        /**
         * Create a particle template used for creating particle system.
         * @param renderer 
         * @param definition 
         */
        constructor(renderer : Renderer, definition : ParticleTemplateDefinition);
        /**
         * Create a new particle system entity instance
         */
        createParticleSystem() : ParticleSystem;
    }
    export class ParticleSystem extends Entity {
        private constructor();
    }

}
type UpAxis = 'x' | 'y' | 'z';

type CameraSettings = {
    up : UpAxis | number[],
    /**
     * Far plane distance, default value is 100
     */
    far? : number;
    /**
     * Near plane distance, default value is 0.1
     */
    near? : number;
    /**
     * Field of view, in degree, default value is 45.
     */
    fov? : number;
    position? : number[],
    lookAt? : number[],
};

type AsposeInit = {
    initialized? : (renderer : Aspose3D.Renderer, renderWindow : Aspose3D.RenderWindow) => void;
    /**
     * The id of the canvas that will be used by the 3D renderer.
     */
    canvas : string;
    /**
     * The A3DW file to be imported.
     */
    url? : string;
    /**
     * Initial camera settings
     */
    camera? : CameraSettings;

    features? : string[];

    /**
     * Center the model to the grid plane, default value is false
     */
    centerModel? : boolean;

    /**
     * Initial variants
     */
    variants? : {[key : string] : any};

    /**
     * The camera's movement style, default value is 'standard'
     */
    movement? : 'standard' | 'orbital' | 'blender' | 'maya' | 'max' | 'fps';
    /**
     * Show a grid, default value is true
     */
    grid? : boolean;
    /**
     * Enable orientation box, default value is true
     */
    orientationBox? : boolean;
};

interface Window {
    aspose3d(init : AsposeInit) : void
}