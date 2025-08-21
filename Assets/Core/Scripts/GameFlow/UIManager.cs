using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI ballsLeftText;
    public TextMeshProUGUI redPegsLeftText;
    public TextMeshProUGUI gamestateText;
    public GameObject winScreen;
    public GameObject loseScreen;
    
    private EntityManager _entityManager;

    private void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
    }

    private void Update()
    {
        var query = _entityManager.CreateEntityQuery(typeof(GameSettings));
        if (query.IsEmpty)
        {
            return;
        }
        
        var gameSettings = query.GetSingleton<GameSettings>();
        
        scoreText.text = "Score: " + gameSettings.currentScore;
        ballsLeftText.text = "Balls: " + gameSettings.ballsRemaining;
        redPegsLeftText.text = "Red Pegs: " + gameSettings.redPegsRemaining;

        switch (gameSettings.gameState)
        {
            case GameState.Playing:
                gamestateText.text = "Playing";
                break;
            case GameState.Bonus:
                gamestateText.text = "Bonus";
                break;
            case GameState.Won:
                gamestateText.text = "You Won!";
                winScreen.SetActive(true);
                break;
            case GameState.Lost:
                gamestateText.text = "You Lost!";
                loseScreen.SetActive(true);
                break;
        }
        
    }

    public void OnReplay()
    {
        // This will destroy all entities and systems in the default world
        if (World.DefaultGameObjectInjectionWorld != null)
        {
            World.DefaultGameObjectInjectionWorld.Dispose();
            DefaultWorldInitialization.Initialize("Default World", false);
        }
        // After reinitialization, reacquire the entity manager
        if (World.DefaultGameObjectInjectionWorld != null)
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void OnExit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
