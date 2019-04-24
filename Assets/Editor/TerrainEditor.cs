using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Terrain))]
public class TerrainEditor : Editor {

	Terrain terrain;
	Editor terrainEditor;
	Editor colorEditor;

	public override void OnInspectorGUI() {
		using ( var check = new EditorGUI.ChangeCheckScope() ) {
			base.OnInspectorGUI();
			if(check.changed){
				terrain.GenerateTerrain();
			}

			if(GUILayout.Button("Generate Terrain")){
				terrain.GenerateTerrain();
			}
			
			DrawSettingsEditor(terrain.terrainSettings, terrain.OnTerrainSettingsUpdated, ref terrain.terrainSettingsFoldout, ref terrainEditor);
			DrawSettingsEditor(terrain.colorSettings, terrain.OnColorSettingsUpdated, ref terrain.colorSettingsFoldout, ref colorEditor);
		}
	}

	void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor){
		if ( settings == null ) return;
		
		foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
		
		using (var check = new EditorGUI.ChangeCheckScope()){
			if(foldout){
				CreateCachedEditor(settings, null, ref editor);
				editor.OnInspectorGUI();

				if(check.changed){
					if(onSettingsUpdated != null){
						onSettingsUpdated();
					}
				}
			}
		}
	}

	private void OnEnable() {
		terrain = (Terrain) target;
	}
}
