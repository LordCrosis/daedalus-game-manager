﻿/* $Id: IGameLogic.cs 819 2011-01-03 01:46:43Z crosis $
 *
 * Description: An implementation of this interface is the game logic necessary 
 * to play a particular game with the Game Manager.
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

namespace DaedalusGameProtocol
{
    public interface IGameLogic
    {
        GameState GetGameState();

        bool IsGameOver();

        void MakeMove(GamePlayer playerNumber, string msg);
    }
}