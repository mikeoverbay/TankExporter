# Tank Exporter

## A tool to export Tanks from World of Tanks.
<a><img src="https://i.imgur.com/OZTVPZr.png" title="source: imgur.com" /></a>

#### This is an ongoing project by me.. Coffee_

## Version 72:
This adds a check box to the extraction screen to extract the tanks item_Def file from the scripts.pkg.
Also... It fixes a bug of items not being found in the shared_content_build file I create for searching for data.

## Version 71:
This fixes a long time issue with importing FBX files with UV vertice counts larger than geometry vertice counts. Texture seams cause problems when rendering or writing the primitive if the vertices are not split based on UV mapping.
The down side to this new FBX reading method is it requires repacking the vertices and indices back in to indexed lists.
This takes time and you will see it when you write out a primitive. The code is not horribly slow but is worth the fix to UV map seams.

## Version 70:
Fixed a bug with loading the Skorpion and other tanks.

## Version 69:
Fixed a issue with the a tanks item_def file messing up finding camouflage texture tile information.
Replaced the season ICONS with edited versions from the game data.

## Version 68:
It would seem Windows 10 does not like having OpenGL render context windows with odd sizes.. This has never been a problem with Windows 7. I have always made the FBO an even size as the rendering textures MUST be powers of 2.
This version resizes the window to a power of 2 after the user manually drags a border causing a resize.
If you are seeing an odd tearing artifact in the center of the window this is because the render window size is not an even number.

I removed a bunch of decals from the terrain. This will speed up rendering a bit. If you are having any issues with the decals showing up as black, please report this along with the OS and Video hardware you are using.

Tank Exporter runs well under SLI (Not tested in crossfire).
Use the NVIDIA control panel to turn SLI on. Add Tank Exporter to the list of programs and than set the SLI setting.
It may take a few trials and error attempts to get the SLI running.

## Version 67:
Added a hack to load the GB93_Caernarvon_AX tank. For what ever reason, this tank has a folder named _skins that causes TE to not find it.. I'm deleting this part of the path so TE can find the primitive files. If you want the new custom textures, copy them over from the _skins folder after extracting the tank to res_mods. When you load the tank in TE, the custom paint should show up.

## Version 66:
This version adds screen capturing to Tank Exporter.
Please read the Help Pages for more information.

## Version 65:
This version adds the ability to load and export crash models from the packages. Editing of the crash models is the same as the regular models. Tank Exporter handles all the paths so you do not need to worry about it. Just load the tank using Load Crashed... on the context menu. Tank Exporter will add _CRASH to the name when the FBX is exported.

## Version 64:
This version adds the ability to edit the Gun models!
You can also add other models to the gun provided you set up the vertex colors correctly.
I spent 2 days 20 + hours working on adding items to the chassis carriage. It can NOT be done.. Adding even one polygon that is not attached to the chassis models with out a matching vertex color causes the shader for the chassis to mess up. The Video driver crashes hard and the game locks up. You can add a ball and attach its vertices to a wheel and it works.. It crashes when there is a model that is not part of the carriage or tracks.

I'm leaving version 63 on GitHub in case there are issues with 64.

## Version 63:
Tank Exporter will now copy and convert the textures of new models that have been added to the Turret or Tank components.
The Visuals will automatically be edited so the paths are changed to match the folder and name of the new textures.

Updated Help pages with this new information.

## Version 62:
After trying a few things to hide the tracks, I think I finally have the solution. In sniper mode, the tracks would show up again even with the ANM map cleared out.
Now.... If you check the Hide Tracks box, Tank Exporter will replaced the primitive(s) for the 'REAL' track segments with a primitive that has only 8 vertices and all the data for each is zeroed out.
Like wise, the rubber band phony tracks vertices are zerored out as well. The normal maps ANM is over written with a blank DDS and the visual is edited to make sure alpha is enabled and the references is at least set to 64. I tested this and I see no artifacts showing up anywhere.

I added code to automatically copy the visual and prmitives files from LOD0 to the other LOD1 to LOD4. You don't have to extract to 1 to 4 as the new check box on the Primitive Writer window will create the folders if they do not exist.

I also updated the HTML help pages with the new changes.


## Version 61:
Created new and much better Help Pages for Tank Exporter.
Added buttons to the Texture View window to hide/show the color channels on the texture. Cleaned up mousing between Texture View window and the rendering window.
Added code to change the user name in texture paths when importing a FBX file. This helps with transporting FBX between friends and others working on the same FBX that is located in folder on a users desktop. The folder names must be exactly the same for this to work correctly.

## Version 60:
Added code to sort the tank list.
Found a bug that was removing the last tank found in each tier while creating the tank list.

## Version 59:
I fixed a bug I managed to add in when I added bloom.
I added "View in res_mods folder" to the Export/Extract menu that will open the tanks folder in Explore. If the folder does not exist, a popup will let you know you have not extracted the files.

## Version 58:
Fixed a issue when visuals and their matching models are scattered in different pkg files. TE should find every part for every tank now.
Bloom now is affected by all 3 lights.
I updated the install banner.

## Version 57:
Fixed a bug loading some of the Polish tanks.
Added Bloom and a way to turn it on and off and a check box to view the texture used for rendering it.

## Verion 56:
This fixes a bug in hiding parts of the tank when a FBX has not been imported.
I also turned FXAA back on and added a check box to disable it.
A few minor bug fixes.

## Version 55:
This changes the texture and other functions to work exactly the same while in fbx view mode. The texture viewer now shows the UVs for the FBX and all mouse over acts just like in model view mode.

Hiding tank parts also works in FBX view now as does vertex colors.

The small tank view window on the bottom right is now presistent.
Loading or Importing a FBX sets this image to that of the current tank and will return to that image after mousing over the list of tanks on the right panel.

The texture names are now displayed in the bottom of the main window and at the top left in the texture view window if you have a texture currently selected.

I fixed a minor but annoying bug with mousing in and out of the texture view window and the main render window. Now it cancels any highlighted UV triangles and stops the confict it was having.

I changed the terrain/grid to a tri-state button... now it cycles through Terrain - Blank - XY Grid displays.


## Version 54:
This adds the ablility to edit the chassis Carriage section.
There are new markers that show where the locations of the wheels/rollers are located.. Moving these, moves the wheels/rollers. There is NO need to move the actual mesh that makes up the wheels. There will be tutorial vidoes on this process coming soon.

I also fixed a few bugs and added a few more features.
1. When importing a FBX, TE no longer reads and data from the res_mods. This has always caused problems with the comparing of the old data and the new FBX.
2. I cleaned up some of the buttons on the side panel and added 2 new ones.. One that shows the vertex colors and one that shows the locations on the BlendBone locations in the chassis visual file.
3. I added a way to hide the track sections of the chassis. It works with many tanks but some may need to have this done by copying images and editing the visual for the chassis.
4. I added a button under Export/Extract that will clean out the data for the current tank in the res_mods folder if any exists.


## Version 53:
Fixed a bug in reading the visual files. Spaces around string names was causing it to not find entries such as "vertices".

## Version 52:
This fixes a crash while extracting the tank data after FBX import.

## Version 51:
Fixed a silly bug that caused the first textures to be blank when exporting to FBX in PNG format.

## Version 50:
TE now exports the currently active camouflage texture when you export the FBX.
Added a camouflage editing tool. Edit Before exporting if you want changes to the camo to be saved.
Moved fbx export and extract functions from the context menu to the main menu.
TE exports the customization.xml file if told to do so.
There is no editing done to the customization.xml. You will need to do it manually if you want camo colors to be visiable in the game.

## Version 49:
The World of Tanks Developers API now requires an Application_Id.
This ID is asscoitated with the Application and the developer of said application. This ID is embeded in Tank Exporters code and there is nothing on your part that is required other than setting up the paths to the game and the current res_mods folder for the current game version.

## Version 48:
Added "Reload Textures" to the context menu on the tier tabs.

## Version 47:
Updated to work with the last version 1.0.2.0 of the game.
Removed version 43 from GitHub. Its unusable with 1.0.2.0.

## Version 46:
Fixed a bug with selection of a UV triangle in the texture view window.

## Version 45:
I added code to clean the shader files of any non-ascii characters. Hopefully, this will help AMD cards load the shaders with no issues.

## Version 44:
I changed how decals are loaded. They are only loaded if they are used. This is dynamic so adding a new decal with an unused texture will cause a slight delay while the decal is being loaded.
This version might cause problems so I am leaving version 42 on GitHub and it should be used if this one fails.
Cleaning out the wot_temp folder might help if the app crashes at startup. Cleaning this folder should be done every time WoT adds or changes tank models!

## Version 43:
Decals are now on the terrain. You can edit and save changes to them. The decals come from the game's pkg files and use PBR to shade them. I am leaving version 42 GitHub incase this version is unusable by anyone.

## Version 42:
Added Shadows and controls for it. You can set 1 of the 3 lights as the shadow light source. Added terrain and skydome. The shader editor now stops the rendering while active. Rendering can be turned back on, on the editors form.
The hide show grid lines now toggels between terrain and grid.
Added a preview of the shadow mapping texture and result. This is a debug tool only.

## Version 41:
Fixed PBR shading.. Added Cube environment map.
Added menu items related to the new shading.

## Version 40:
Fixed a issue with writing the surface normals X flipped

## Version 39:
Added a FBO to use with FXAA.
Implemented FXAA.
Improved PBR and lighting in general.

## Version 38:
I Finally got a working PBR shader. Better lighting!

## Version 37:
Fixed an excpetion being thrown when mousing out of the Texture View window.

### Version 36:
Added a bar graph during the extarction of large game files so TE doesn't look like its crashed.

### Version 35:
Exporting to the res_mods folder now has an option to create a Work Area and copy the AM_HD and AO_HD to that folder in PNG format. This saves having to do it maually. You may not need to edit the AO map depending on what your doing but most likely, if you are painting on the AM map, you will need to fix the alpha change in the AO map.
When using the Texture Viewer and Show UVs is on.. mousing over triangles on the texture now highlights them in the model view window. This will make it easier to figure out where the UVs are located on the tank. The method of mousing over the tank model still works as before and highlights the UV map where that triangle is mapped.


### Version 34:
Added type icons to the tank list.
Added Component load window to select which gun and turret that were found for the tank.
Fixed a minor bug in the shader for the tanks.
Updated the help HTML.

### Version 33:
I fixed a bug with the HD textures never actually being loaded or exported to the FBX folder. I'm amazed I didn't catch this eariler.

### Version 32:
I Fixed a bug reported by vontamar at GitHub.. Normals were being written with the X flipped! THANK YOU! Reporting bugs makes TE a better tool!

### Version 32:
I fixed a bug caused by new content pgk names that stop the loading of HD textures.

### Version 30:
I reworked how tanks are found in the PKG files.
As of 4/8/2018 TE is finding 549 tanks.

### Version 29:
Added code to load the Italy Tank line.
There is currently only one tank in this line.. A preimium.
This data is not in the WoT API site so I hard coded the name for the It13_Pregetto_M35_mod_46

### Verion 28:
Removed BSP and BSP Tree from the menu.. They are no longer part of primitive files.
Fixed coding to deal with part1 and part2 content files.
I have not added code to deal with the Italy line yet so these tanks are NOT loaded in to the list.
I removed writing BSP2 data to the prmitive files as these are now contained in VT files (I think... I need to do more exploring of these files formats).

### Version 27:
Added a Simple Lighting mode for those that are having problems with rendering the tanks.
If you are having issues with textures not showing up, please create a ticket explaining the issue and video hardware you are using

### Version 26:
Changed where the Terra tank list and data is stored because of UAC problems. Updated the tank list for Terra.

### Version 25:
This version fixes a memory leak. Updated the tank list for Terra.

### Version 24:
Tank Exporter now loads all tanks in the extended pkg files.

### Version 23:
Fixed a stupid bug in the path names to where the game and res_mods.
Added some code to find more of the missing tank parts.
WG has split the tier 8 tanks in to 2 pgk files. I will need to change the code to deal with this as some point. As it is, tanks from that 2nd pkg are ignore.

### Version 22:
Paths to folders are now saved to the wot_temp folder.
You won't need to reset the game and res_mods/currentversion
paths after every update to Tank Exporter.

### Version 21:
Added a shader while viewing the FBX.
A few minor big fixes with loading textures from the res_mods folder path.

### Version 20:
This fixes a path issue when texture paths reference the a old game version folder. It nows repaces that path with the current game version path to the res_mods folder.

### Version 19:
Added support for adding more than one one object to the turret or hull.

### Version 17:
Fixed a nasty issue with the normals of added items.
They were not being translated correctly.

### Version 18:
More fixes in FBX import/export

### Version 16:
Fixes in FBX import/expore

### Version 15:
Fixed a issue while writing Turret models.
Added a model info window.
Added / Fixed Visual viewer window.
Now TE seaches in the res_mods folder for a matching tank component. If it finds it, will load it from there.
Minor Bug fixes.

### Version 14:
Added Primitive Write capability.
Now you can write primitive files the game can load!
This only works will the hull or turret models.
Read the help file for more information.

### Version 13:
Added Mouse over picking of the vertex under the mouse.
This only works if UVs are visable in the Texture Viewer.
Also.. Pressing the "C" key will center the current UV in the Texture Viewer's window.
The purpose is to make it easy to locate where the UVs are mapped to.
Updated Help HTML and added a new page and image.

### Version 12:
Added Texture Viewing and UV ploting.. also a way to save the texture as a png. This is dependent on the view settings.
Added importing of the BSP2 and the tree from the tank models.

### Version 11:
Added FBX importing. Re did the User Interface. 

### Verions 10:
Now when exporting a tank in FBX, a folder is created under the tanks name in the same directory and all the textures are placed there.