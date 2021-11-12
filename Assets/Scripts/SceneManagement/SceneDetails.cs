using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] private List<SceneDetails> connectedScenes;
    public bool IsLoaded { get; private set; }

    private List<SavableEntity> savableEntities;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log($"Entered {gameObject.name}!");

            LoadScene();
            GameController.Instance.SetCurrentScene(this);

            //Load all connected scenes
            foreach (var scene in connectedScenes)
            {
                scene.LoadScene();
            }

            //Unload the scene that are no longer connected
            var prevScene = GameController.Instance.PreviousScene;
            if (prevScene != null)
            {
                var previouslyLoadedScenes = prevScene.connectedScenes;
                foreach (var scene in previouslyLoadedScenes)
                {
                    if (!connectedScenes.Contains(scene) && scene != this)
                    {
                        scene.UnloadScene();
                    }
                }

                if (!connectedScenes.Contains(prevScene))
                    prevScene.UnloadScene();
            }
        }
    }

    public void LoadScene()
    {
        if (!IsLoaded)
        {
            var loading = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;
            loading.completed += (AsyncOperation op) =>
            {
                savableEntities = GetSavableEntitiesInScene();
                SavingSystem.i.RestoreEntityStates(savableEntities);
            };
        }
    }

    public void UnloadScene()
    {
        if (IsLoaded)
        {
            SavingSystem.i.CaptureEntityStates(savableEntities);

            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
        }
    }

    private List<SavableEntity> GetSavableEntitiesInScene()
    {
        var currentScene = SceneManager.GetSceneByName(gameObject.name);
        var savableEntities = FindObjectsOfType<SavableEntity>().Where(x => x.gameObject.scene == currentScene).ToList();
        return savableEntities;
    }
}