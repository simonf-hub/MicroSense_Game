using UnityEngine;

namespace MicroSense
{
    public sealed class PrototypeHud : MonoBehaviour
    {
        [SerializeField] private GameManager _gameManager;

        private GUIStyle _panelStyle;
        private GUIStyle _playerStyle;
        private GUIStyle _centerStyle;
        private GUIStyle _objectiveStyle;
        private GUIStyle _gemStyle;
        private GUIStyle _completeStyle;
        private Texture2D _panelTexture;

        private void OnGUI()
        {
            if (_gameManager == null || _gameManager.Players == null || _gameManager.Players.Length < 2)
            {
                return;
            }

            EnsureStyles();

            const float margin = 14f;
            float topHeight = _gameManager.MaxAbilitiesPerPlayer > 1 || _gameManager.TotalGems > 0 ? 84f : 62f;
            float availableWidth = Mathf.Max(320f, Screen.width - margin * 2f);
            float playerWidth = Mathf.Clamp(availableWidth * 0.28f, 170f, 330f);
            float centerWidth = Mathf.Max(160f, availableWidth - playerWidth * 2f - margin * 2f);

            DrawPlayerPanel(new Rect(margin, margin, playerWidth, topHeight), _gameManager.Players[0]);
            DrawCenterPanel(new Rect(margin + playerWidth + margin, margin, centerWidth, topHeight));
            DrawPlayerPanel(
                new Rect(Screen.width - margin - playerWidth, margin, playerWidth, topHeight),
                _gameManager.Players[1]);

            GUI.Label(
                new Rect(margin, Screen.height - 38f, Screen.width - margin * 2f, 24f),
                "Reset [R]",
                _objectiveStyle);

            if (_gameManager.LevelComplete)
            {
                Rect overlay = new Rect(Screen.width * 0.5f - 230f, Screen.height * 0.5f - 72f, 460f, 144f);
                GUI.Box(overlay, GUIContent.none, _panelStyle);
                GUI.Label(new Rect(overlay.x + 20f, overlay.y + 24f, overlay.width - 40f, 48f), "LEVEL COMPLETE", _completeStyle);
                GUI.Label(new Rect(overlay.x + 20f, overlay.y + 82f, overlay.width - 40f, 28f), "Both cells reached the membrane gate", _objectiveStyle);
            }
        }

        private void DrawPlayerPanel(Rect rect, PlayerController player)
        {
            GUI.color = player.PlayerColor;
            GUI.Box(rect, GUIContent.none, _panelStyle);
            GUI.color = Color.white;
            GUI.Label(new Rect(rect.x + 14f, rect.y + 8f, rect.width - 28f, 22f), player.PlayerName, _playerStyle);

            PlayerAbilityController abilities = player.AbilityController;
            string primary = FormatAbilitySlot(abilities.PrimaryAbility, player.InputBindings.AbilityPrimary);
            GUI.Label(new Rect(rect.x + 14f, rect.y + 33f, rect.width - 28f, 22f), primary, _playerStyle);

            if (_gameManager.MaxAbilitiesPerPlayer > 1)
            {
                string secondary = abilities.UnlockedAbilitySlots < 2
                    ? $"SLOT 2: LOCKED [{player.InputBindings.AbilitySecondary}]"
                    : FormatAbilitySlot(abilities.SecondaryAbility, player.InputBindings.AbilitySecondary, 2);
                GUI.Label(new Rect(rect.x + 14f, rect.y + 54f, rect.width - 28f, 22f), secondary, _playerStyle);
            }
        }

        private static string FormatAbilitySlot(AbilityType ability, UnityEngine.InputSystem.Key key, int slot = 1)
        {
            string value = ability == AbilityType.None
                ? "EMPTY"
                : ability.ToString().ToUpperInvariant();
            return $"SLOT {slot}: {value} [{key}]";
        }

        private void DrawCenterPanel(Rect rect)
        {
            GUI.color = new Color(0.65f, 0.95f, 0.82f);
            GUI.Box(rect, GUIContent.none, _panelStyle);
            GUI.color = Color.white;
            GUI.Label(new Rect(rect.x + 10f, rect.y + 7f, rect.width - 20f, 21f), _gameManager.LevelName, _centerStyle);
            GUI.Label(new Rect(rect.x + 10f, rect.y + 30f, rect.width - 20f, 21f), _gameManager.Objective, _objectiveStyle);
            if (_gameManager.TotalGems > 0)
            {
                GUI.Label(
                    new Rect(rect.x + 10f, rect.y + 54f, rect.width - 20f, 21f),
                    $"GEMS  {_gameManager.CollectedGems} / {_gameManager.TotalGems}",
                    _gemStyle);
            }
        }

        private void EnsureStyles()
        {
            if (_panelStyle != null)
            {
                return;
            }

            _panelTexture = new Texture2D(1, 1);
            _panelTexture.SetPixel(0, 0, new Color(0.035f, 0.055f, 0.08f, 0.9f));
            _panelTexture.Apply();

            _panelStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { background = _panelTexture }
            };
            _playerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = Color.white }
            };
            _centerStyle = new GUIStyle(_playerStyle)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 15
            };
            _objectiveStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.82f, 0.9f, 0.93f) }
            };
            _gemStyle = new GUIStyle(_objectiveStyle)
            {
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.45f, 0.86f, 1f) }
            };
            _completeStyle = new GUIStyle(_centerStyle)
            {
                fontSize = 28,
                normal = { textColor = new Color(0.65f, 1f, 0.82f) }
            };
        }

        public void Configure(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        private void OnDestroy()
        {
            if (_panelTexture != null)
            {
                Destroy(_panelTexture);
            }
        }
    }
}
