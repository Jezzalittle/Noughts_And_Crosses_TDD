using Noughts_And_Crosses_TDD;
using NUnit.Framework;

NoughtsAndCrosses game = new NoughtsAndCrosses();
game.OnTurnChanged += () =>
{
	string input = Console.ReadLine();
	Console.Clear();
	game.MakeMove(game.ValidMoveFromString(input), game.ActivePlayer);

	if(game.Winner != State.Empty)
	{
		Console.Clear();
		Console.WriteLine("{0} is the winner!!!!!!", game.Winner);
	}
	else
	{
		Console.Clear();
		Console.WriteLine("Stalemate!");
	}

};
game.CreateGame();





