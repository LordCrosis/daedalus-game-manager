﻿/* $Id$
 *
 * Description: The game class contains the game state and the logic for making
 * moves and checking the status of the game.
 *
 * Copyright (c) 2010, Team Daedalus (Mathew Bergt, Jason Buck, Ken Kelley, and
 * Justin Weaver).
 *
 * Distributed under the BSD-new license. For details see the BSD_LICENSE file
 * that should have been included with this distribution. If the source you
 * acquired this distribution from incorrectly removed this file, the license
 * may be viewed at http://www.opensource.org/licenses/bsd-license.php.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DaedalusGameProtocol;
using TzaarGame;

namespace TzaarGame
{
    public class TzaarLogic : IGameLogic
    {
        // The board map.
        private TzaarBoardMap boardMap;

        // The game state.
        private TzaarGameState state;

        // A game can be constructed without a parameter (empty board), with a
        // premade board, with a premade state, or with the bool value 'true'
        // for a randomly generated board.
        public TzaarLogic()
        {
            this.boardMap = new TzaarBoardMap();
            TzaarBoard board = new TzaarBoard();
            this.state = new TzaarGameState(board);
        }

        public TzaarLogic(GameBoard aBoard)
        {
            this.boardMap = new TzaarBoardMap();
            this.state = new TzaarGameState(aBoard);
        }

        public TzaarLogic(GameState aState)
        {
            this.boardMap = new TzaarBoardMap();
            this.state = (TzaarGameState)aState;
        }

        // Return a reference to the current game state.
        public GameState GetGameState()
        {
            return state;
        }

        // Perform a Pass move.
        private bool Pass()
        {
            // This message is a Pass command.
            if (this.state.IsFirstMoveOfTurn)
                // Can't pass your first turn; you lose.
                return false;

            // The command was successful!
            return true;
        }

        // Checks if the game is over.  If it is over, then this method sets
        // the Game Over condition.
        private void CheckForGameOver()
        {
            bool whiteCanCapture = this.WhiteCanCapture();
            bool blackCanCapture = this.BlackCanCapture();

            if (whiteCanCapture && !blackCanCapture)
            {
                // White can capture, and black cannot; white won.
                this.state.SetGameOver(GamePlayer.One, GameOverCondition.YouWin);
            }
            else if (blackCanCapture && !whiteCanCapture)
            {
                // Black can capture, and white cannot; black won.
                this.state.SetGameOver(GamePlayer.Two, GameOverCondition.YouWin);
            }
            else if (!whiteCanCapture && !blackCanCapture)
            {
                // Neither side can make a move.  The rules state that a
                // player loses when they can't make any more moves.  If
                // this is the player's first move of the turn, they can
                // pass their second move.   If it is their second move,
                // then the opposing player goes next.  Either way, the
                // current player wins.
                this.state.SetGameOver(this.state.GetCurrentPlayerNumber(), GameOverCondition.YouWin);
            }
        }

        // Convenience overload.
        public void Move(int fromCol, int fromRow, int toCol, int toRow)
        {
            Move(new TzaarMessage.Move(fromCol, fromRow, toCol, toRow));
        }

        // Execute a move.
        private void Move(TzaarMessage.Move m)
        {
            Stack<TzaarPiece> source = this.state.Board.Query(m.FromCol, m.FromRow);
            Stack<TzaarPiece> destination = this.state.Board.Query(m.ToCol, m.ToRow);

            // Check for invalid source and destination coordinates.
            if (source == null || destination == null)
                throw new Exception("Invalid source or destination position.");

            // Check if the source or destination spaces are empty.
            if (source.Count == 0 || destination.Count == 0)
                throw new Exception("Invalid source or destination position are empty.");

            TzaarColor sourceColor = source.Peek().Color;
            TzaarColor destinationColor = destination.Peek().Color;

            if (sourceColor != PlayerColor.GetColorFromPlayer(this.state.GetCurrentPlayerNumber()))
                // You can't move these pieces, they belong to the other player.
                throw new Exception("Invalid source pieces belong to other player.");

            // Check for a valid path between the two spaces.
            if (!this.boardMap.IsValidPath(this.state.Board, m.FromCol, m.FromRow, m.ToCol, m.ToRow))
                throw new Exception("There is no valid path between the source and destination positions.");

            if (this.state.IsFirstMoveOfTurn)
            {
                // Check that the pieces are of different colors.
                if (sourceColor == destinationColor)
                    throw new Exception("The first move of a turn must be a capture, but the pieces in the source and designation positions have the same color.");

                // We can't capture a stack that is more powerful than us.
                if (source.Count < destination.Count)
                    throw new Exception("The stack in the destination position is more powerful than the source stack.");

                Capture(m.FromCol, m.FromRow, m.ToCol, m.ToRow);
            }
            else
            {
                // If the pieces are of different colors, we capture; otherwise
                // we stack.
                if (sourceColor != destinationColor)
                {
                    // We can't capture a stack that is more powerful than us.
                    if (source.Count < destination.Count)
                        throw new Exception("The stack in the destination position is more powerful than the source stack.");

                    Capture(m.FromCol, m.FromRow, m.ToCol, m.ToRow);
                }
                else
                {
                    Stack(m.FromCol, m.FromRow, m.ToCol, m.ToRow);
                }
            }

            CheckForGameOver();
        }

        // Perform a stack move.
        private void Stack(int fromCol, int fromRow, int toCol, int toRow)
        {
            // Take the piece from the source position.
            Stack<TzaarPiece> S = this.state.Board.Take(fromCol, fromRow);

            // Add the pieces that were in the source position to the stack at
            // the destination position.
            this.state.Board.Add(S, toCol, toRow);
        }

        // Perform a capture move.
        private void Capture(int fromCol, int fromRow, int toCol, int toRow)
        {
            // Take the pieces from the source position.
            Stack<TzaarPiece> S = this.state.Board.Take(fromCol, fromRow);

            // Take the pieces from the destination position.
            Stack<TzaarPiece> capturedStack = this.state.Board.Take(toCol, toRow);

            // Now, add the pieces that were in the source position to the
            // destination position.
            this.state.Board.Add(S, toCol, toRow);
        }

        // Return true if the specified color is able to make a capture using
        // any one of their stacks.
        private bool FooCanCapture(TzaarColor playerColor)
        {
            // The opponent's piece color.
            TzaarColor targetColor = (playerColor == TzaarColor.BLACK) ? TzaarColor.WHITE : TzaarColor.BLACK;

            // Check each spot on the board, looking for our pieces.  If we find
            // one, we check to see if we can use it to capture any of the other
            // player's pieces.
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Stack<TzaarPiece> S = this.state.Board.Query(i, j);
                    if (S == null)
                        // The columns have varying length.  Switch to the next
                        // column as soon as we reach the top of the current
                        // one.
                        break;
                    if (S.Count == 0)
                        // There are no pieces on this position, keep going.
                        continue;
                    if (S.Peek().Color != playerColor)
                        // These are the other player's pieces, keep going.
                        continue;

                    // We have found some of our pieces!  Ok, now we check each
                    // position on the board to see if we can move there.
                    // Brute force, FTW!
                    for (int k = 0; k < 9; k++)
                    {
                        for (int l = 0; l < 9; l++)
                        {
                            Stack<TzaarPiece> SS = this.state.Board.Query(k, l);
                            if (SS == null)
                                break;
                            if (SS.Count == 0)
                                continue;
                            if (SS.Peek().Color != targetColor)
                                continue;
                            // Found a potential target; is it wimpy?
                            if (SS.Count > S.Count)
                                continue;
                            // Can we reach the potential target?
                            if (this.boardMap.IsValidPath(this.state.Board, i, j, k, l))
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        // Return true if the white player is able to make a capture.
        public bool WhiteCanCapture()
        {
            return FooCanCapture(TzaarColor.WHITE);
        }

        // Return true if the black player is able to make a capture.
        public bool BlackCanCapture()
        {
            return FooCanCapture(TzaarColor.BLACK);
        }

        // Return true if white has run out of one of the piece-types.
        public bool WhiteIsOutOfPieces()
        {
            return (this.state.Board.WhiteTzaarCount == 0 || this.state.Board.WhiteTzarraCount == 0 || this.state.Board.WhiteTottCount == 0);
        }

        // Return true if black has run out of one of the piece-types.
        public bool BlackIsOutOfPieces()
        {
            return (this.state.Board.BlackTzaarCount == 0 || this.state.Board.BlackTzarraCount == 0 || this.state.Board.BlackTottCount == 0);
        }

        // Return true if the game state alone tells us that the game is over.
        public bool IsGameOver()
        {
            return (WhiteIsOutOfPieces() || BlackIsOutOfPieces() || !WhiteCanCapture() || !BlackCanCapture());
        }

        public void MakeMove(GamePlayer playerNumber, string msg)
        {
            // Parse it as a move command.
            TzaarMessage.Move move;
            try
            {
                move = new TzaarMessage.Move(msg);
            }
            catch (Exception)
            {
                // Junk message.
                throw new Exception(playerNumber + " forfeits, because they sent a move command that could not be parsed.");
            }

            // Now execute the command we received.
            if (move.IsPass)
            {
                // Make the requested Pass.
                if (!Pass())
                {
                    // The attempt to Pass failed!
                    throw new Exception(playerNumber + " forfeits, because they attempted an illegal Pass.");
                }
            }
            else
            {
                // Make the requested move.
                try
                {
                    Move(move);
                }
                catch (Exception ex)
                {
                    // The move attempt failed!
                    throw new Exception(playerNumber + "Move Failed: " + move + "  Reason: " + ex.Message);
                }
            }
        }
    }
}