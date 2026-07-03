using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneController : MonoBehaviour
{
    public static MenuSceneController Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {

    }

    private void Update()
    {

    }

        public void OnButton_EnterGameClick()
    {
        SceneManager.LoadScene("01-GameScene"); // Jump to GameScene
    }
}

