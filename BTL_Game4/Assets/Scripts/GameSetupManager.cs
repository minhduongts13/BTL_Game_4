using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System;

public class GameSetupManager : MonoBehaviour
{
    public static GameSetupManager Instance { get; private set; }

    public int AIlevel = 1;
    [Header("UNOUI")]
    public GameObject unoContainer;  // toàn bộ panel UNO (có thể kèm Text hướng dẫn)
    public Button unoButton;         // button “UNO”
    public float unoTimeout = 2f;    // thời gian chờ bấm (giây)
    private bool _unoPressed;
    [Header("UI")]
    public ColorWheelController colorWheel;
    public GameBoardPanel gameBoardPanel;

    [Header("Setup")]
    public List<OpponentData> opponentsData; // Dữ liệu mỗi đối thủ

    [Header("References")]
    public PlayerHandManager playerHandManager;
    public GameObject opponentHandPrefab;
    public Transform opponentsParent;
    public List<OpponentHandManager> opponentManagers = new();
    [Header("Deck UI")]
    public Text deckCountText;
    public PlayedCardsManager playedCardsManager;  // Gán trong Inspector
    public List<string> AllPlayers = new();
    public int direction = 1;
    public string turn;
    //public Dictionary<string, List<CardData>> hands = new Dictionary<string, List<CardData>>();
    public Dictionary<string, HandManagerBase> hands = new Dictionary<string, HandManagerBase>();
    public DrawPileClickHandler DrawClick;
    public int numplayers;
    public static System.Random rng = new System.Random();



    // Hàm chuyển đổi enum sang vị trí cố định (có thể điều chỉnh theo bố cục của bạn)
    Vector3 GetPositionForOpponent(OpponentPosition pos)
    {
        switch (pos)
        {
            case OpponentPosition.Top:
                return new Vector3(0, 300, 0);
            case OpponentPosition.Right:
                return new Vector3(500, 0, 0);
            case OpponentPosition.Left:
                return new Vector3(-500, 0, 0);
            default:
                return Vector3.zero;
        }
    }

    // Tính góc xoay để đối thủ "hướng về" trung tâm (Center = (0,0,0))
    float GetRotationForPosition(OpponentPosition pos)
    {
        Vector3 posVec = GetPositionForOpponent(pos);
        Vector2 dir = (Vector2.zero - new Vector2(posVec.x, posVec.y)).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return angle + 90;  // Điều chỉnh nếu cần
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);   // (tuỳ chọn) giữ manager qua scene
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        CreateOpponents();
        DealInitialCards();
    }

    void Start()
    {
        if (colorWheel != null && gameBoardPanel != null)
        {
            colorWheel.OnColorSelected += (Color selectedColor) =>
            {
                gameBoardPanel.SetBackgroundColor(selectedColor);
            };
        }
        MergePlayer();
        //Test();
        // tắt UI UNO lúc đầu
        unoContainer.SetActive(false);

        // gán listener cho button
        unoButton.onClick.AddListener(OnUnoButtonClicked);
    }

    void CreateOpponents()
    {
        foreach (var data in opponentsData)
        {
            Debug.Log($"Tạo đối thủ: {data.opponentName} ở vị trí: {data.fixedPosition}");

            GameObject newOpponent = Instantiate(opponentHandPrefab, opponentsParent);

            // Đặt vị trí và xoay dựa trên giá trị của enum trong OpponentData
            Vector3 targetPos = GetPositionForOpponent(data.fixedPosition);
            newOpponent.transform.localPosition = targetPos;
            newOpponent.transform.rotation = Quaternion.Euler(0, 0, GetRotationForPosition(data.fixedPosition));

            // Sử dụng GetComponentInChildren để lấy Component từ con của prefab
            OpponentHandManager manager = newOpponent.GetComponentInChildren<OpponentHandManager>();
            manager.position = data.fixedPosition;
            OpponentUI ui = newOpponent.GetComponentInChildren<OpponentUI>();

            if (ui != null)
            {
                // Gọi hàm Setup để truyền thông tin trong OpponentData
                ui.Setup(data);
                Debug.Log($"Đã setup UI cho đối thủ: {data.opponentName}");

                // Điều chỉnh vị trí infoPanel nếu cần (ví dụ: đặt sang bên trái)
                if (ui.infoPanel != null)
                {
                    RectTransform handRect = manager.opponentHandPanel as RectTransform;
                    RectTransform infoRect = ui.infoPanel; // Cái này đã là RectTransform rồi

                    if (handRect != null && infoRect != null)
                    {
                        Vector2 handPos = handRect.anchoredPosition;
                        infoRect.anchoredPosition = new Vector2(handPos.x + 250f, handPos.y + 50f);
                    }
                    else
                    {
                        Debug.LogWarning("Không thể ép kiểu sang RectTransform!");
                    }

                }
            }
            else
            {
                Debug.LogWarning("Không tìm thấy OpponentUI trên prefab!");
            }

            if (manager != null)
            {
                opponentManagers.Add(manager);
                Debug.Log($"OpponentHandManager đã được thêm cho: {data.opponentName}");
            }
            else
            {
                Debug.LogWarning("Không tìm thấy OpponentHandManager trên prefab!");
            }
        }

        Debug.Log($"Tổng số đối thủ được tạo: {opponentManagers.Count}");
    }

    void DealInitialCards()
    {
        Debug.Log("Chia bài cho người chơi...");
        for (int i = 0; i < 7; i++)
        {
            var card = DeckManager.Instance.DrawCard();
            if (card != null)
            {
                playerHandManager.SpawnCard(card);
                // Có thể thêm Debug.Log(card.cardName) nếu cần
            }
        }

        Debug.Log("Chia bài cho đối thủ...");
        foreach (var opponent in opponentManagers)
        {
            for (int i = 0; i < 7; i++)
            {
                var card = DeckManager.Instance.DrawCard();
                if (card != null)
                {
                    opponent.SpawnCard(card);  // Đây sẽ tạo lá bài úp
                    Debug.Log("Đối thủ nhận lá bài (úp).");
                }
            }
        }
        Debug.Log("Chia bài hoàn tất.");
        //Debug.Log(playerHandManager.playerCards[0]);
        while (true)
        {
            CardData card_first = DeckManager.Instance.DrawCard();
            // Nếu là số thì lấy làm lá đầu, kiểm tra bằng TryParse
            if (card_first.cardNumber >= 0 && card_first.cardNumber <= 9)
            {
                playedCardsManager.AddPlayedCard(card_first);
                break;
            }
            else
            {
                // Nếu là lá đặc biệt thì bỏ lại cuối bộ bài
                DeckManager.Instance.AddDeck(card_first);
                Debug.Log("HAhahahahahahahahah.");
            }
        }
        numplayers = 1 + opponentManagers.Count;
        //UpdateDeckCount(); // ← Cập nhật số bài sau khi chia
    }

    void UpdateDeckCount()
    {
        if (deckCountText != null)
        {
            deckCountText.text = DeckManager.Instance.RemainingCardCount().ToString();
        }
    }

    void MergePlayer()
    {
        AllPlayers.Clear();
        AllPlayers.Add("Player");

        if (opponentsData.Exists(o => o.fixedPosition == OpponentPosition.Left))
            AllPlayers.Add("AI1");

        if (opponentsData.Exists(o => o.fixedPosition == OpponentPosition.Top))
            AllPlayers.Add("AI2");

        if (opponentsData.Exists(o => o.fixedPosition == OpponentPosition.Right))
            AllPlayers.Add("AI3");

        turn = AllPlayers[0];

        // Gán các đối tượng hand vào dictionary theo thứ tự (Player và các AI)
        hands.Clear();
        hands["Player"] = playerHandManager;
        foreach (var opponent in opponentManagers)
        {
            switch (opponent.position)
            {
                case OpponentPosition.Left:
                    hands["AI1"] = opponent;
                    break;
                case OpponentPosition.Top:
                    hands["AI2"] = opponent;
                    break;
                case OpponentPosition.Right:
                    hands["AI3"] = opponent;
                    break;
            }
        }

        foreach (var opponent in AllPlayers)
        {
            Debug.Log(opponent);
        }

    }

    public bool isValidCard(CardData cardData)
    {
        string current_player = turn;
        if (current_player != "Player") return false;
        CardData topCard = playedCardsManager.playedCards[playedCardsManager.playedCards.Count - 1];
        return cardData.Matches(topCard);
    }

    public void SwitchTurn(string status, CardData lastCard = null)
    {
        int current_index = this.AllPlayers.IndexOf(this.turn);
        int new_index = (current_index + this.direction + this.numplayers) % this.numplayers;
        //Debug.Log(lastCard.cardNumber);
        if (lastCard != null)
        {
            if (status == "draw" || status == "UNO")
            {
                this.turn = this.AllPlayers[new_index];
                return;
            }
            // Using if-else instead of match/case from Python
            if (lastCard.cardNumber == -10)
            {
                //Console.WriteLine($"{this.players[new_index]} is banned!");
                new_index = (new_index + this.direction + this.numplayers) % this.numplayers;
            }
            else if (lastCard.cardNumber == -11)
            {
                this.direction *= -1;
                new_index = (current_index + this.direction + this.numplayers) % this.numplayers;
                if (this.numplayers == 2)
                {
                    new_index = current_index; // Với 2 người, "rev" giống "skip"
                }
            }
            else if (lastCard.cardNumber == -12 || lastCard.cardNumber == -13)
            {
                // Get number from second character of card.number string
                int numcard = lastCard.cardNumber == -12 ? 2 : 4;
                for (int i = 0; i < numcard; i++)
                {
                    hands[this.AllPlayers[new_index]].SpawnCard(DeckManager.Instance.DrawCard());
                }
                //Console.WriteLine($"{this.Allplayers[new_index]} draw {numcard} cards!");
                new_index = (new_index + this.direction + this.numplayers) % this.numplayers;
            }
        }

        this.turn = this.AllPlayers[new_index];
        Debug.Log(new_index);
        Debug.Log(direction);
        //Debug.Log(hands[this.AllPlayers[new_index]]);
    }

    IEnumerator UnoPenaltyRoutine()
    {
        _unoPressed = false;
        unoContainer.SetActive(true);

        float timer = 0f;
        while (timer < unoTimeout && !_unoPressed)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        unoContainer.SetActive(false);

        if (!_unoPressed)
        {
            Debug.Log("Bạn không bấm UNO kịp → rút 2 lá phạt");
            playerHandManager.SpawnCard(DeckManager.Instance.DrawCard());
            playerHandManager.SpawnCard(DeckManager.Instance.DrawCard());
        }
        else
        {
            Debug.Log("Bạn đã bấm UNO kịp!");
        }
        SwitchTurn("UNO", playedCardsManager.playedCards[playedCardsManager.playedCards.Count - 1]);
    }

    /// <summary>
    /// Gọi từ Button.OnClick
    /// </summary>
    public void OnUnoButtonClicked()
    {
        _unoPressed = true;
    }
    // Biến cờ để đảm bảo không xử lý nhiều AI cùng lúc
    private bool aiProcessing = false;

    void Update()
    {
        // Nếu đến lượt của AI (không phải AllPlayers[0]) và không đang xử lý di chuyển
        if (!aiProcessing && this.turn != AllPlayers[0])
        {
            StartCoroutine(ProcessAIMove());
        }
        foreach (var player in hands)
        {
            if (player.Value.playerCards.Count == 0)
            {
                Debug.Log("Người chiến thắng");
            }
        }
    }

    IEnumerator ProcessAIMove()
    {
        aiProcessing = true;

        // Lấy đối tượng tay bài của AI, ép kiểu về OpponentHandManager
        OpponentHandManager phm = hands[this.turn] as OpponentHandManager;
        if (phm == null)
        {
            Debug.LogError("Không tìm thấy OpponentHandManager cho " + this.turn);
            aiProcessing = false;
            yield break;
        }

        // Đợi 1 giây trước khi AI suy nghĩ (có thể chỉnh thời gian tùy theo mong muốn)
        yield return new WaitForSeconds(1f);

        // Lấy nước đi của AI
        if (AIlevel == 0)
        {
            CardData aiMove = AIDecision(phm);
            if (aiMove == null)
            {
                // Nếu không có nước đi phù hợp, AI sẽ rút bài
                phm.SpawnCard(DeckManager.Instance.DrawCard());
                // Chờ thêm chút thời gian để hiển thị việc rút bài
                yield return new WaitForSeconds(0.5f);

                // Lấy lá bài vừa được thêm vào tay (giả sử là phần tử cuối của danh sách)
                CardData extraCard = phm.playerCards.Last();
                // Nếu lá bài này khớp với lá bài vừa đánh, nó trở thành nước đi của AI
                if (extraCard.Matches(playedCardsManager.playedCards.Last()))
                {
                    aiMove = extraCard;
                }
            }

            if (aiMove != null)
            {
                // Nếu lá bài là 'wild', chọn màu ngẫu nhiên
                if (aiMove.cardColor == "wild")
                {
                    aiMove.cardColor = new string[] { "red", "yellow", "blue", "green" }[rng.Next(4)];
                }
                // Thêm lá bài vào vùng bài đã đánh
                playedCardsManager.AddPlayedCard(aiMove);

                // Xóa lá bài khỏi tay AI
                phm.RemoveCardObject(aiMove);

                // Chuyển lượt, truyền thông tin nước đi của AI
                SwitchTurn("discard", aiMove);
            }
            else
            {
                // Nếu không có nước đi phù hợp, chuyển lượt với hành động "draw"
                SwitchTurn("draw", aiMove);
            }

            // Đợi 1 giây nữa sau khi xử lý để chuyển cảnh rõ ràng
            yield return new WaitForSeconds(1f);

            aiProcessing = false;
        }
        else
        {
            Debug.Log("Hhahahahahahahahahahah");
            (SimState currentstate, CardData aiMove) = MonteCarloDecision(phm);
            if (aiMove == null)
            {
                // Nếu không có nước đi phù hợp, AI sẽ rút bài
                phm.SpawnCard(DeckManager.Instance.DrawCard());
                // Chờ thêm chút thời gian để hiển thị việc rút bài
                yield return new WaitForSeconds(0.5f);
                // Lấy lá bài vừa được thêm vào tay (giả sử là phần tử cuối của danh sách)
                CardData extraCard = phm.playerCards.Last();
                // Nếu lá bài này khớp với lá bài vừa đánh, nó trở thành nước đi của AI
                if (should_play_drawn_card(currentstate, extraCard))
                {
                    aiMove = extraCard;
                }
            }

            if (aiMove != null)
            {
                // Nếu lá bài là 'wild', chọn màu ngẫu nhiên
                if (aiMove.cardColor == "wild")
                {
                    aiMove.cardColor = best_color_choice(currentstate);
                }
                // Thêm lá bài vào vùng bài đã đánh
                playedCardsManager.AddPlayedCard(aiMove);

                // Xóa lá bài khỏi tay AI
                phm.RemoveCardObject(aiMove);

                // Chuyển lượt, truyền thông tin nước đi của AI
                SwitchTurn("discard", aiMove);
            }
            else
            {
                // Nếu không có nước đi phù hợp, chuyển lượt với hành động "draw"
                SwitchTurn("draw", aiMove);
            }

            // Đợi 1 giây nữa sau khi xử lý để chuyển cảnh rõ ràng
            yield return new WaitForSeconds(1f);

            aiProcessing = false;
        }
    }

    // Giả sử AIDecision nhận đối số là OpponentHandManager để có thể truy cập tay bài của AI
    public CardData AIDecision(OpponentHandManager phm)
    {
        // Lấy bài cuối cùng của vùng bài đã đánh để so sánh, dùng Last của System.Linq
        CardData lastPlayed = playedCardsManager.playedCards.Last();
        // Tìm nước đi hợp lệ từ tay của AI
        var legalMoves = phm.playerCards.Where(card => card.Matches(lastPlayed)).ToList();
        if (legalMoves.Count == 0)
        {
            return null; // Nghĩa là "DRAW"
        }
        else
        {
            return legalMoves[rng.Next(legalMoves.Count)];
        }
    }



    // --- Các biến AI ---
    //private bool aiProcessing = false;
    public float aiDelayBeforeMove = 1f;
    public float aiDelayAfterMove = 1f;

    //#region MONTE CARLO INTEGRATION

    /// <summary>
    /// Lớp SimState: chứa trạng thái cần thiết cho mô phỏng
    /// (Dữ liệu được xây dựng từ các thành phần hiện có: AllPlayers, turn, hands, playedCardsManager)
    /// </summary>
    public class SimState
    {
        public List<CardData> deck;
        public Dictionary<string, List<CardData>> simuhands;
        public List<CardData> discardPile;
        //public List<CardData> draw_pile;
        public List<string> forced_winner;
        public string Turn;
        public int Direction; // 1: clockwise, -1: counterclockwise
        public List<string> players;
        //public static Random rng;

        public SimState()
        {
            players = new List<string>();
            simuhands = new Dictionary<string, List<CardData>>();
            discardPile = new List<CardData>();
            deck = new List<CardData>();
            Turn = "";
            Direction = 1;
            forced_winner = new List<string>();
        }

        // Deep copy bao gồm cả bộ bài
        public SimState DeepCopy()
        {
            SimState copy = new SimState();
            copy.players = new List<string>(this.players);
            copy.Turn = this.Turn;
            copy.discardPile = new List<CardData>(this.discardPile);
            copy.simuhands = new Dictionary<string, List<CardData>>();
            copy.Direction = this.Direction;
            foreach (var kvp in this.simuhands)
            {
                // Giả sử CardData có thể copy qua tham chiếu (nếu không, cần copy các thuộc tính riêng)
                copy.simuhands[kvp.Key] = new List<CardData>(kvp.Value);
            }
            copy.deck = new List<CardData>(this.deck);
            copy.forced_winner = new List<string>(this.forced_winner);
            return copy;
        }

        /// <summary>
        /// Rút bài từ bộ bài mô phỏng
        /// </summary>
        public void DrawCard(string player)
        {
            if (deck.Count > 0)
            {
                CardData drawn = deck[0];
                deck.RemoveAt(0);
                if (!simuhands.ContainsKey(player))
                    simuhands[player] = new List<CardData>();
                simuhands[player].Add(drawn);
            }
            else
            {
                // forced_winner is determined if no more cards to draw
                this.forced_winner = this.players.OrderBy(p => this.simuhands[p].Count).ToList();
            }
        }

        public void PlayCard(string player, CardData card)
        {
            if (simuhands.ContainsKey(player) && simuhands[player].Contains(card))
            {
                simuhands[player].Remove(card);
                discardPile.Add(card);
            }
        }

        public void Switchturn(string status, CardData lastCard = null)
        {
            int current_index = this.players.IndexOf(this.Turn);
            int new_index = (current_index + this.Direction + players.Count) % players.Count;

            if (lastCard != null)
            {
                if (status == "draw" || status == "UNO")
                {
                    this.Turn = this.players[new_index];
                    return;
                }
                // Using if-else instead of match/case from Python
                if (lastCard.cardNumber == -10)
                {
                    //Console.WriteLine($"{this.players[new_index]} is banned!");
                    new_index = (new_index + this.Direction + players.Count) % players.Count;
                }
                else if (lastCard.cardNumber == -11)
                {
                    this.Direction *= -1;
                    new_index = (current_index + this.Direction + players.Count) % players.Count;
                    if (players.Count == 2)
                    {
                        new_index = current_index; // Với 2 người, "rev" giống "skip"
                    }
                }
                else if (lastCard.cardNumber == -12 || lastCard.cardNumber == -13)
                {
                    // Get number from second character of card.number string
                    int numcard = lastCard.cardNumber == -12 ? 2 : 4;
                    for (int i = 0; i < numcard; i++)
                    {
                        this.DrawCard(this.players[new_index]);
                    }
                    //Console.WriteLine($"{this.Allplayers[new_index]} draw {numcard} cards!");
                    new_index = (new_index + this.Direction + players.Count) % players.Count;
                }
            }
            this.Turn = this.players[new_index];
        }

        public bool IsGameOver()
        {
            foreach (var hand in this.simuhands.Values)
            {
                if (hand.Count == 0)
                {
                    return true;
                }
            }
            return this.forced_winner.Count != 0;
        }

        public List<string> Winner()
        {
            if (this.forced_winner.Count == 0)
            {
                this.forced_winner = this.players.OrderBy(p => this.simuhands[p].Count).ToList();
            }
            return this.forced_winner;
        }
    }


    /// <summary>
    /// Lớp Node dùng trong MCTS.
    /// </summary>
    public class Node
    {
        public SimState state;
        public Node parent;
        public object move; // CardData hoặc "DRAW"
        public List<Node> children;
        public int visits;
        public double wins;
        public List<object> untriedMoves;
        public string rootPlayer;
        public static System.Random rng = new System.Random();

        public Node(SimState state, Node parent = null, object move = null, string rootPlayer = null)
        {
            this.state = state;
            this.parent = parent;
            this.move = move;
            this.children = new List<Node>();
            this.visits = 0;
            this.wins = 0;
            this.untriedMoves = GetLegalMoves();
            this.rootPlayer = rootPlayer ?? state.Turn;
        }

        public List<object> GetLegalMoves()
        {
            List<object> legalMoves = new List<object>();
            List<CardData> currentHand = state.simuhands[state.Turn];
            CardData topCard = state.discardPile.Last();
            foreach (CardData card in currentHand)
            {
                if (card.Matches(topCard))
                    legalMoves.Add(card);
            }
            if (legalMoves.Count == 0)
                legalMoves.Add("DRAW");
            return legalMoves;
        }

        public double Ucb1()
        {
            if (visits == 0)
                return double.PositiveInfinity;
            return (wins / visits) + Math.Sqrt(2 * Math.Log(parent.visits) / visits);
        }

        public Node Select()
        {
            if (untriedMoves.Count > 0)
                return Expand();
            else
                return children.OrderByDescending(c => c.Ucb1()).First();
        }

        public Node Expand()
        {
            if (untriedMoves.Count == 0)
                return children[rng.Next(children.Count)];
            object move = untriedMoves[untriedMoves.Count - 1];
            untriedMoves.RemoveAt(untriedMoves.Count - 1);
            SimState newState = SimulateMove(move);
            Node child = new Node(newState, this, move);
            children.Add(child);
            return child;
        }

        public SimState SimulateMove(object move)
        {
            SimState newState = state.DeepCopy();
            string currentPlayer = newState.Turn;
            if (move is string s && s == "DRAW")
            {
                newState.DrawCard(currentPlayer);
                newState.Switchturn("draw", null);
            }
            else if (move is CardData card)
            {
                newState.PlayCard(currentPlayer, card);
                newState.Switchturn("discard", card);
            }

            return newState;
        }

        public double Simulate(int maxDepth = 50)
        {
            SimState simState = state.DeepCopy();
            int simulationDepth = 0;
            while (!simState.IsGameOver())
            {
                if (simulationDepth >= maxDepth)
                    return Heuristic.Evaluate(simState, rootPlayer);
                simulationDepth++;
                string currentPlayer = simState.Turn;
                CardData topCard = simState.discardPile.Last();
                List<object> legalMoves = new List<object>();
                foreach (CardData card in simState.simuhands[currentPlayer])
                {
                    if (card.Matches(topCard))
                        legalMoves.Add(card);
                }
                object move = legalMoves.Count > 0 ? legalMoves[rng.Next(legalMoves.Count)] : "DRAW";
                if (move is string s2 && s2 == "DRAW")
                {
                    simState.DrawCard(currentPlayer);
                    simState.Switchturn("draw", null);
                }
                else if (move is CardData cardMove)
                {
                    simState.PlayCard(currentPlayer, cardMove);
                    simState.Switchturn("discard", cardMove);
                }

            }
            List<string> ranking = simState.Winner();
            int rootIndex = ranking.IndexOf(rootPlayer);
            int playerIndex = ranking.IndexOf("Player"); // Giả sử tên người chơi là "Player"
            return (rootIndex < playerIndex) ? 1 : 0;
        }

        public void Backpropagate(double result)
        {
            visits++;
            wins += result;
            if (parent != null)
                parent.Backpropagate(result);
        }
    }

    public static class MCTS
    {
        public static (object bestMove, Node root) mctsDecision(SimState state, int simulations = 1000, int maxDepth = 50)
        {
            Node root = new Node(state, rootPlayer: state.Turn);
            for (int i = 0; i < simulations; i++)
            {
                Node node = root.Select();
                double outcome = node.Simulate(maxDepth);
                node.Backpropagate(outcome);
            }
            Node bestChild = root.children.OrderByDescending(c => c.visits).FirstOrDefault();
            object bestMove = bestChild != null ? bestChild.move : null;
            return (bestMove, root);
        }
    }

    public static class Heuristic
    {
        // Đánh giá trạng thái dựa trên số bài của rootPlayer so với đối thủ.
        public static double Evaluate(SimState state, string rootPlayer)
        {
            int rootCount = state.simuhands[rootPlayer].Count;
            List<int> opponentCounts = new List<int>();
            foreach (string opp in state.players)
            {
                if (opp != rootPlayer)
                    opponentCounts.Add(state.simuhands[opp].Count);
            }
            if (opponentCounts.Count == 0) return 1.0;
            int minOpp = opponentCounts.Min();
            if (rootCount < minOpp)
                return 1.0;
            else if (rootCount > minOpp)
                return 0.0;
            else
                return 0.5;
        }
    }
    public static bool should_play_drawn_card(SimState game_state, CardData drawn_card, int simulations = 500)
    {
        // Nếu lá bài không hợp lệ, không thể đánh.
        if (!drawn_card.Matches(game_state.discardPile.Last()))
        {
            return false;
        }

        // Tạo hai phiên bản game để mô phỏng
        SimState play_state = game_state.DeepCopy();
        SimState hold_state = game_state.DeepCopy();

        string current_player = game_state.Turn;
        // Mô phỏng kịch bản 1: Đánh lá bài mới rút
        play_state.simuhands[current_player].RemoveAll(c => c.Equals(drawn_card));
        play_state.discardPile.Add(drawn_card);
        play_state.Switchturn("discard", drawn_card);
        var (dummy1, play_root) = MCTS.mctsDecision(play_state, simulations, 50);

        // Mô phỏng kịch bản 2: Giữ bài, rút lượt tiếp theo
        var (dummy2, hold_root) = MCTS.mctsDecision(hold_state, simulations, 50);

        double play_win_rate = 0;
        if (play_root.children.Count > 0)
        {
            play_win_rate = play_root.children.Max(child => child.visits > 0 ? child.wins / child.visits : 0);
        }
        double hold_win_rate = 0;
        if (hold_root.children.Count > 0)
        {
            hold_win_rate = hold_root.children.Max(child => child.visits > 0 ? child.wins / child.visits : 0);
        }
        return play_win_rate >= hold_win_rate;
    }

    public static string best_color_choice(SimState game)
    {
        Dictionary<string, double> color_win_rates = new Dictionary<string, double>
            {
                { "red", 0 },
                { "yellow", 0 },
                { "blue", 0 },
                { "green", 0 }
            };
        foreach (string color in color_win_rates.Keys.ToList())
        {
            SimState simulated_game = game.DeepCopy();
            simulated_game.discardPile.Last().cardColor = color;
            var (dummy, root) = MCTS.mctsDecision(simulated_game, simulations: 500);
            double max_rate = 0;
            foreach (Node child in root.children)
            {
                double rate = child.visits > 0 ? child.wins / child.visits : 0;
                if (rate > max_rate)
                {
                    max_rate = rate;
                }
            }
            color_win_rates[color] = max_rate;
        }
        // Return the color with best win rate.
        return color_win_rates.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
    }
    private (SimState state, CardData card) MonteCarloDecision(OpponentHandManager phm)
    {
        SimState currentState = BuildSimState();
        (object best_move, Node root) = MCTS.mctsDecision(currentState, simulations: 10000, maxDepth: 1000);
        if (best_move is string s && s.Equals("DRAW"))
            return (currentState, null);
        else if (best_move is CardData cd)
            return (currentState, cd);
        else
        {
            Debug.LogWarning("MCTS không trả về nước đi hợp lệ, AI sẽ rút bài");
            return (currentState, null);
        }
    }

    /// <summary>
    /// Xây dựng SimState từ dữ liệu hiện tại của game.
    /// Lấy AllPlayers, turn, playedCards, và các tay bài từ hands.
    /// </summary>
    /// <returns>SimState</returns>
    private SimState BuildSimState()
    {
        SimState state = new SimState();
        state.players = AllPlayers.ToList();
        state.Turn = turn;
        state.Direction = direction;
        state.discardPile = new List<CardData>(playedCardsManager.playedCards);
        state.simuhands = new Dictionary<string, List<CardData>>();
        foreach (string player in AllPlayers)
        {
            // Giả sử mỗi HandManagerBase có property playerCards kiểu List<CardData>
            state.simuhands[player] = new List<CardData>(hands[player].playerCards);
        }
        // Lấy bản copy của deck từ DeckManager
        state.deck = DeckManager.Instance.GetDeckCopy();

        return state;
    }


}
