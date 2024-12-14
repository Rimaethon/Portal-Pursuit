using Data;
using Managers;
using TMPro;
using UnityEngine;

	public class UIPlayerStateView:MonoBehaviour
	{
		[SerializeField] private RectTransform healthBar;
		[SerializeField] private TextMeshProUGUI playerScoreView;
		[SerializeField] private GameObject playerLivePrefab;
		private int playerLives;
		private int playerScore;
		private int highScore;
		private UserData userData;

		private void Awake()
		{
			userData=SaveManager.Instance.GetUserData();
			playerLives=userData.Lives;
			HandlePlayerLivesChanged(playerLives);
			playerScoreView.text = "0";
		}

		private void HandlePlayerLivesChanged(int newLives)
		{
			foreach (Transform child in healthBar)
			{
				Destroy(child.gameObject);
			}
			for (int i = 0; i < newLives; i++)
			{
				Instantiate(playerLivePrefab, healthBar);
			}
		}
	}
