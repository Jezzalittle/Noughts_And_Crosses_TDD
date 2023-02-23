using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Noughts_And_Crosses_TDD
{
	public enum ValidMove
	{
		Invalid,
		TL,
		TM,
		TR,
		ML,
		MM,
		MR,
		BL,
		BM,
		BR
	}

	public enum State
	{
		Empty,
		O,
		X
	}

	[Serializable]
	public class InvalidMoveException : Exception
	{
		public InvalidMoveException(string message)
			: base(message) { }

		public InvalidMoveException(string message, Exception inner)
			: base(message, inner) { }
	}

	public class NoughtsAndCrosses
	{
		#region Static
		public static string BOARD_FORMAT = "{0} | {1} | {2}\n-----------\n{3} | {4} | {5}\n-----------\n{6} | {7} | {8}";
		#endregion

		#region Variables
		private Dictionary<ValidMove, State> boardState = new Dictionary<ValidMove, State>();
		public delegate void OnTurnChangedDelegate();
		public OnTurnChangedDelegate? OnTurnChanged;
		#endregion

		#region Properties
		public int MovesCounter { get; private set; } = 0;
		public State ActivePlayer { get; private set;} = State.Empty;
		public State Winner { get; private set; } = State.Empty;
		public Dictionary<ValidMove, State> BoardState { get => boardState; }
		#endregion
		#region Public Functions
		public void CreateGame()
		{
			//loop over all the vaild moves and create an empty state
			//start at 1 to skip invalid 
			var validMoves = Enum.GetValues<ValidMove>();
			for (int i = 1; i < validMoves.Length; i++)
			{
				SetBoardState(validMoves[i], State.Empty);
			}
			LogVaildMovesString();
			Console.Write("\n\n\n");
			LogGameState();
			ActivePlayerCoinFlip();
			if(OnTurnChanged != null)
			{
				OnTurnChanged();
			}
		}

		public void MakeMove(ValidMove move, State state)
		{
			if (move == ValidMove.Invalid)
			{
				throw new InvalidMoveException("Invalid move, vaild moves:\n" + GetVaildMovesString());
			}

			if (BoardState[move] != State.Empty)
			{
				throw new InvalidMoveException("Not an empty position, vaild moves:\n" + GetVaildMovesString());
			}

			if(state == State.Empty)
			{
				throw new InvalidMoveException("Players can't set positions to empty");
			}

			SetBoardState(move, state);
			if(CheckForWinner() || CheckForStalemate())
			{
				return;
			}
			ChangeActivePlayersTurn();
			MovesCounter++;
			LogVaildMovesString();
			Console.Write("\n\n\n");
			LogGameState();
			LogActivePlayer();
			if (OnTurnChanged != null)
			{
				OnTurnChanged();
			}
		}

		public string GetVaildMovesString()
		{
			string[] vaildStrings = new string[9];

			for (int i = 0; i < boardState.Count; i++)
			{
				var keyValuePair = boardState.ElementAt(i);
				vaildStrings[i] = keyValuePair.Value == State.Empty ? keyValuePair.Key.ToString() : keyValuePair.Value.ToString();
			}
			return String.Format(BOARD_FORMAT, vaildStrings[0], vaildStrings[1], vaildStrings[2], vaildStrings[3], vaildStrings[4], vaildStrings[5], vaildStrings[6], vaildStrings[7], vaildStrings[8]);
		}
		public string GetGameStateString()
		{
			string[] vaildStrings = new string[9];

			for (int i = 0; i < boardState.Count; i++)
			{
				var keyValuePair = boardState.ElementAt(i);
				vaildStrings[i] = keyValuePair.Value == State.Empty ? " " : keyValuePair.Value.ToString();
			}
			return String.Format(BOARD_FORMAT, vaildStrings[0], vaildStrings[1], vaildStrings[2], vaildStrings[3], vaildStrings[4], vaildStrings[5], vaildStrings[6], vaildStrings[7], vaildStrings[8]);
		}

		public ValidMove ValidMoveFromString(string moveString)
		{
			ValidMove validMove = ValidMove.Invalid;
			if (Enum.TryParse<ValidMove>(moveString, true, out validMove))
			{
				return validMove;
			}
			return ValidMove.Invalid;
		}

		#endregion

		#region Private Functions
		private void SetBoardState(ValidMove validMove, State state)
		{
			boardState[validMove] = state;
		}

		private void ActivePlayerCoinFlip()
		{
			ActivePlayer = (State)new Random().Next(1,2);
			LogActivePlayer();
		}

		private void LogActivePlayer()
		{
			Console.WriteLine("\nIt's {0} turn", ActivePlayer.ToString());
		}

		private void ChangeActivePlayersTurn()
		{
			ActivePlayer = (State)((int)ActivePlayer == 1 ? 2 : 1);
		}

		private void LogGameState()
		{
			Console.Write(GetGameStateString());
		}

		private void LogVaildMovesString()
		{
			Console.Write(GetVaildMovesString());
		}

		private bool CheckForWinner()
		{
			for (int i = 1; i < 3; i++)
			{
				State state = (State)i;
				//check columns
				if ((boardState[ValidMove.TL] == state && boardState[ValidMove.ML] == state && boardState[ValidMove.BL] == state) //Left column
					|| (boardState[ValidMove.TM] == state && boardState[ValidMove.MM] == state && boardState[ValidMove.BM] == state) //Middle column
					|| (boardState[ValidMove.TR] == state && boardState[ValidMove.MR] == state && boardState[ValidMove.BR] == state)) //Right column
				{
					Winner = state;
					return true;
				}

				//Check rows
				if ((boardState[ValidMove.TL] == state && boardState[ValidMove.TM] == state && boardState[ValidMove.TR] == state) //Top row
					|| (boardState[ValidMove.ML] == state && boardState[ValidMove.MM] == state && boardState[ValidMove.MR] == state) //Middle row
					|| (boardState[ValidMove.BL] == state && boardState[ValidMove.BM] == state && boardState[ValidMove.BR] == state)) //Bottom row
				{
					Winner = state;
					return true;
				}

				//Check Diagonal
				if ((boardState[ValidMove.TL] == state && boardState[ValidMove.MM] == state && boardState[ValidMove.BR] == state) //Top row
					|| (boardState[ValidMove.TR] == state && boardState[ValidMove.MM] == state && boardState[ValidMove.BL] == state)) //Bottom row
				{
					Winner = state;
					return true;
				}
			}
			return false;
		}
		private bool CheckForStalemate()
		{
			foreach (var keyValuePair in boardState)
			{
				if(keyValuePair.Value == State.Empty)
				{
					return false;
				}
			}
			return true;
		}
		#endregion
	}

	public class NoughtsAndCrossesTests
	{
		[Test]
		public void CreateGame_ZeroMoves()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			Assert.AreEqual(0, game.MovesCounter);
		}
		
		[Test]
		public void CreateGame_CheckBoardString()
		{
			Assert.AreEqual("TL | TM | TR\n-----------\nML | MM | MR\n-----------\nBL | BM | BR", String.Format(NoughtsAndCrosses.BOARD_FORMAT, "TL", "TM", "TR", "ML", "MM", "MR", "BL", "BM", "BR"));
		}

		[Test]
		public void CreateGame_CheckBoardSize()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			Assert.IsTrue(game.BoardState.Count == 9);
		}

		[Test]
		public void CreateGame_CheckBoardState()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			bool isEmpty = true;
			foreach (var keyValuePair in game.BoardState)
			{
				if(keyValuePair.Value != State.Empty)
				{
					isEmpty = false;
				}
			}
			Assert.IsTrue(isEmpty);
		}

		[Test]
		public void CreateGame_PlayersTurnCoinFlip()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			Assert.IsTrue(game.ActivePlayer != State.Empty);
		}

		[Test]
		public void MakeMove_CounterShift()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			game.MakeMove(ValidMove.TL, State.X);

			Assert.AreEqual(1, game.MovesCounter);
		}

		[Test]
		public void MakeMove_CheckValidMoveString()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			game.MakeMove(ValidMove.TL, State.X);
			Assert.AreEqual("X | TM | TR\n-----------\nML | MM | MR\n-----------\nBL | BM | BR", game.GetVaildMovesString());
		}

		[Test]
		public void MakeMove_CheckPlayersTurnChanaged()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			State player = game.ActivePlayer;
			game.MakeMove(ValidMove.TL, State.X);
			Assert.AreNotEqual(player, game.ActivePlayer);
		}

		[Test]
		public void MakeMove_CheckPlayersTurnIsNotEmpty()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			game.MakeMove(ValidMove.TL, State.X);
			Assert.IsTrue(game.ActivePlayer != State.Empty);
		}

		[Test]
		public void MakeMove_InvalidMoveThrowException()
		{
			Assert.Throws<InvalidMoveException>(() =>
			{
				NoughtsAndCrosses game = new NoughtsAndCrosses();
				game.CreateGame();
				ValidMove move = game.ValidMoveFromString("X");
				game.MakeMove(move, State.O);
			});
		}
		
		[Test]
		public void MakeMove_EmptyMoveThrowException()
		{
			Assert.Throws<InvalidMoveException>(() =>
			{
				NoughtsAndCrosses game = new NoughtsAndCrosses();
				game.CreateGame();
				game.MakeMove(ValidMove.TL, State.Empty);
			});
		}

		[Test]
		public void MakeMove_MoveOnExistingPositionThrowException()
		{
			Assert.Throws<InvalidMoveException>(() =>
			{
				NoughtsAndCrosses game = new NoughtsAndCrosses();
				game.CreateGame();
				game.MakeMove(ValidMove.TL, State.O);
				game.MakeMove(ValidMove.TL, State.X);
			});
		}

		[Test]
		public void MakeMove_CheckGameStateString()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			game.MakeMove(ValidMove.TL, State.O);
			Assert.AreEqual("O |   |  \n-----------\n  |   |  \n-----------\n  |   |  ", game.GetGameStateString());
		}

		[Test]
		public void WinCondition_CheckLeftColumnO()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			game.MakeMove(ValidMove.TL, State.O);
			game.MakeMove(ValidMove.TM, State.X);
			game.MakeMove(ValidMove.ML, State.O);
			game.MakeMove(ValidMove.MM, State.X);
			game.MakeMove(ValidMove.BL, State.O);
			Assert.IsTrue(game.Winner == State.O);
		}

		[Test]
		public void WinCondition_CheckMiddleColumnO()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			game.MakeMove(ValidMove.TM, State.O);
			game.MakeMove(ValidMove.TL, State.X);
			game.MakeMove(ValidMove.MM, State.O);
			game.MakeMove(ValidMove.ML, State.X);
			game.MakeMove(ValidMove.BM, State.O);
			Assert.IsTrue(game.Winner == State.O);
		}

		[Test]
		public void WinCondition_CheckRightmColumnO()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			game.MakeMove(ValidMove.TR, State.O);
			game.MakeMove(ValidMove.TM, State.X);
			game.MakeMove(ValidMove.MR, State.O);
			game.MakeMove(ValidMove.MM, State.X);
			game.MakeMove(ValidMove.BR, State.O);
			Assert.IsTrue(game.Winner == State.O);
		}

		[Test]
		public void WinCondition_CheckLeftColumnX()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			game.MakeMove(ValidMove.TL, State.X);
			game.MakeMove(ValidMove.TM, State.O);
			game.MakeMove(ValidMove.ML, State.X);
			game.MakeMove(ValidMove.MM, State.O);
			game.MakeMove(ValidMove.BL, State.X);
			Assert.IsTrue(game.Winner == State.X);
		}

		[Test]
		public void WinCondition_CheckMiddleColumnX()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			game.MakeMove(ValidMove.TM, State.X);
			game.MakeMove(ValidMove.TL, State.O);
			game.MakeMove(ValidMove.MM, State.X);
			game.MakeMove(ValidMove.ML, State.O);
			game.MakeMove(ValidMove.BM, State.X);
			Assert.IsTrue(game.Winner == State.X);
		}

		[Test]
		public void WinCondition_CheckRightmColumnX()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			game.MakeMove(ValidMove.TR, State.X);
			game.MakeMove(ValidMove.TM, State.O);
			game.MakeMove(ValidMove.MR, State.X);
			game.MakeMove(ValidMove.MM, State.O);
			game.MakeMove(ValidMove.BR, State.X);
			Assert.IsTrue(game.Winner == State.X);
		}

		[Test]
		public void WinCondition_CheckTopRowO()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			game.MakeMove(ValidMove.TL, State.O);
			game.MakeMove(ValidMove.ML, State.X);
			game.MakeMove(ValidMove.TM, State.O);
			game.MakeMove(ValidMove.MM, State.X);
			game.MakeMove(ValidMove.TR, State.O);
			Assert.IsTrue(game.Winner == State.O);
		}
		
		[Test]
		public void WinCondition_CheckMiddleRowO()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			game.MakeMove(ValidMove.ML, State.O);
			game.MakeMove(ValidMove.TL, State.X);
			game.MakeMove(ValidMove.MM, State.O);
			game.MakeMove(ValidMove.TM, State.X);
			game.MakeMove(ValidMove.MR, State.O);
			Assert.IsTrue(game.Winner == State.O);
		}

		[Test]
		public void WinCondition_CheckBottomRowO()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			game.MakeMove(ValidMove.BL, State.O);
			game.MakeMove(ValidMove.ML, State.X);
			game.MakeMove(ValidMove.BM, State.O);
			game.MakeMove(ValidMove.MM, State.X);
			game.MakeMove(ValidMove.BR, State.O);
			Assert.IsTrue(game.Winner == State.O);
		}

		[Test]
		public void WinCondition_CheckTopRowX()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			game.MakeMove(ValidMove.TL, State.X);
			game.MakeMove(ValidMove.ML, State.O);
			game.MakeMove(ValidMove.TM, State.X);
			game.MakeMove(ValidMove.MM, State.O);
			game.MakeMove(ValidMove.TR, State.X);
			Assert.IsTrue(game.Winner == State.X);
		}

		[Test]
		public void WinCondition_CheckMiddleRowX()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			game.MakeMove(ValidMove.ML, State.X);
			game.MakeMove(ValidMove.TL, State.O);
			game.MakeMove(ValidMove.MM, State.X);
			game.MakeMove(ValidMove.TM, State.O);
			game.MakeMove(ValidMove.MR, State.X);
			Assert.IsTrue(game.Winner == State.X);
		}

		[Test]
		public void WinCondition_CheckBottomRowX()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			game.MakeMove(ValidMove.BL, State.X);
			game.MakeMove(ValidMove.ML, State.O);
			game.MakeMove(ValidMove.BM, State.X);
			game.MakeMove(ValidMove.MM, State.O);
			game.MakeMove(ValidMove.BR, State.X);
			Assert.IsTrue(game.Winner == State.X);
		}

		[Test]
		public void WinCondition_CheckTLBRDiagonalLineO()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			game.MakeMove(ValidMove.TL, State.O);
			game.MakeMove(ValidMove.ML, State.X);
			game.MakeMove(ValidMove.MM, State.O);
			game.MakeMove(ValidMove.BL, State.X);
			game.MakeMove(ValidMove.BR, State.O);
			Assert.IsTrue(game.Winner == State.O);
		}

		[Test]
		public void WinCondition_CheckTRBLDiagonalLineO()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			game.MakeMove(ValidMove.TR, State.O);
			game.MakeMove(ValidMove.ML, State.X);
			game.MakeMove(ValidMove.MM, State.O);
			game.MakeMove(ValidMove.MR, State.X);
			game.MakeMove(ValidMove.BL, State.O);
			Assert.IsTrue(game.Winner == State.O);
		}

		[Test]
		public void WinCondition_CheckTLBRDiagonalLineX()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			game.MakeMove(ValidMove.TL, State.X);
			game.MakeMove(ValidMove.ML, State.O);
			game.MakeMove(ValidMove.MM, State.X);
			game.MakeMove(ValidMove.BL, State.O);
			game.MakeMove(ValidMove.BR, State.X);
			Assert.IsTrue(game.Winner == State.X);
		}

		[Test]
		public void WinCondition_CheckTRBLDiagonalLineX()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			game.MakeMove(ValidMove.TR, State.X);
			game.MakeMove(ValidMove.ML, State.O);
			game.MakeMove(ValidMove.MM, State.X);
			game.MakeMove(ValidMove.MR, State.O);
			game.MakeMove(ValidMove.BL, State.X);
			Assert.IsTrue(game.Winner == State.X);
		}

		[Test]
		public void WinCondition_CheckStalemate()
		{
			NoughtsAndCrosses game = new NoughtsAndCrosses();
			game.CreateGame();
			game.MakeMove(ValidMove.TL, State.O);
			game.MakeMove(ValidMove.TR, State.X);
			game.MakeMove(ValidMove.TM, State.O);
			game.MakeMove(ValidMove.ML, State.X);
			game.MakeMove(ValidMove.MR, State.O);
			game.MakeMove(ValidMove.MM, State.X);
			game.MakeMove(ValidMove.BL, State.O);
			game.MakeMove(ValidMove.BR, State.X);
			game.MakeMove(ValidMove.BM, State.O);
			Assert.IsTrue(game.Winner == State.Empty);
		}

	}
}
