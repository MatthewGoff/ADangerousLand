using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    public GameObject GrassPrefab;
    public GameObject FogPrefab;
    public GameObject PlayerPrefab;

    public int WorldWidth;
	public int WorldHeight;
	
	private GameObject[,] Grass;
    private GameObject[,] Fog;
	private GameObject Player;

	public void CreateWorld() {
		CreateGrass();
        //CreateFog();
		CreatePlayer();
	}

    private void CreateFog() {
        Fog = new GameObject[WorldWidth, WorldHeight];
		for (int x=0; x<WorldWidth; x++) {
			for (int y=0; y<WorldHeight; y++) {
				Fog[x,y] = Instantiate(FogPrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                Fog[x,y].GetComponent<FogController>().AssignPlayer(Player);
			}
		}
    }

    private void CreateGrass() {
        Grass = new GameObject[WorldWidth, WorldHeight];
		for (int x=0; x<WorldWidth; x++) {
			for (int y=0; y<WorldHeight; y++) {
                Grass[x,y] = Instantiate(GrassPrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
			}
		}
    }

	private void CreatePlayer() {
        Player = Instantiate (PlayerPrefab, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
	}

	public void DestoryWorld() {
		if (Player != null) {
            Object.Destroy(Player);
        }
        if (Fog != null) {
			for (int x=0; x<WorldWidth; x++) {
				for (int y=0; y<WorldHeight; y++) {
					Object.Destroy(Fog[x,y]);
				}
			}
		}
        if (Grass != null) {
			for (int x=0; x<WorldWidth; x++) {
				for (int y=0; y<WorldHeight; y++) {
					Object.Destroy(Grass[x,y]);
				}
			}
		}
	}

	public GameObject GetPlayer() {
		return Player;
	}
}
