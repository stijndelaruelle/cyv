using UnityEngine;
using System.Collections;

public class Fog : MonoBehaviour
{
    [SerializeField]
    GameObject m_FogParticleEffect;

	private void Awake ()
    {
        GameplayManager.Instance.OnNewGame += OnNewGame;
        GameplayManager.Instance.OnChangePlayer += OnChangePlayer;
        GameplayManager.Instance.OnChangeGameState += OnChangeGameState;
	}
	
    private void OnDestroy()
    {
        if (GameplayManager.Instance != null)
        {
            GameplayManager.Instance.OnNewGame -= OnNewGame;
            GameplayManager.Instance.OnChangePlayer -= OnChangePlayer;
            GameplayManager.Instance.OnChangeGameState -= OnChangeGameState;
        }
    }

    private void OnNewGame()
    {
        //Enable ourselves
        m_FogParticleEffect.SetActive(true);
        gameObject.GetComponent<RectTransform>().rotation = Quaternion.identity;
    }

    private void OnChangePlayer(PlayerColor player)
    {
        //Turn 180 degrees
        if (GameplayManager.Instance.GameMode == GameMode.PassAndPlay)
            return;

        if (GameplayManager.Instance.NumAIPlayers() > 0)
            return;

        if (m_FogParticleEffect.activeSelf)
        {
            gameObject.GetComponent<RectTransform>().Rotate(new Vector3(0.0f, 0.0f, 180.0f));
        }
    }

    private void OnChangeGameState(GameState currentGameState, GameState prevGameState)
    {
        //Hide ourselves
        if (prevGameState == GameState.Setup)
        {
            m_FogParticleEffect.SetActive(false);
        }
    }
}
