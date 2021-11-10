using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] private List<SceneDetails> connectedScenes;
    public bool IsLoaded { get; private set; }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log($"Entered {gameObject.name}!");

            LoadScene();
            GameController.Instance.SetCurrentScene(this);

            //Load all connected scenes
            foreach(var scene in connectedScenes)
            {
                scene.LoadScene();
            }

            //Unload the scene that are no longer connected
            if (GameController.Instance.PreviousScene != null)
            {
                var previouslyLoadedScenes = GameController.Instance.PreviousScene.connectedScenes;
                foreach(var scene in previouslyLoadedScenes)
                {
                    if(!connectedScenes.Contains(scene) && scene != this)
                    {
                        scene.UnloadScene();
                    }
                }
            }
        }
    }

    public void LoadScene()
    {
        if (!IsLoaded)
        {
            SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;
        }
    }

    public void UnloadScene()
    {
        if (IsLoaded)
        {
            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
        }
    }
}