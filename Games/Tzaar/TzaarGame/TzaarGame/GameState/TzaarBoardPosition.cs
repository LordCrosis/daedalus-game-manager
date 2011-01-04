﻿/* $Id$
 *
 * Description: And TzaarBoard.Position object represents a position on the
 * TzaarBoard.  It contains all necessary information about the state of that
 * position (how many pieces are on it, and what those pieces are).
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

namespace TzaarGame
{
    public class TzaarBoardPosition
    {
        private Stack<TzaarPiece> pieces;

        // Constructor initializes a new stack to hold potential game pieces.
        public TzaarBoardPosition()
        {
            this.pieces = new Stack<TzaarPiece>();
        }

        // Add a game piece to the stack at this position.
        public void Add(TzaarPiece aPiece)
        {
            this.pieces.Push(aPiece);
        }

        // Add multiple game pieces to the stack at this position.
        public void Add(Stack<TzaarPiece> somePieces)
        {
            for (int i = somePieces.Count - 1; i >= 0; i--)
                this.pieces.Push(somePieces.ElementAt(i));
        }

        // Remove and return all game pieces at this position, if any.
        public Stack<TzaarPiece> Take()
        {
            Stack<TzaarPiece> SS = this.pieces;
            this.pieces = new Stack<TzaarPiece>();
            return SS;
        }

        // Returns information about any pieces contained at this position.
        public Stack<TzaarPiece> Query()
        {
            Stack<TzaarPiece> SS = new Stack<TzaarPiece>();
            for (int i = this.pieces.Count - 1; i >= 0; i--)
                SS.Push(this.pieces.ElementAt(i));
            return SS;
        }
    }
}