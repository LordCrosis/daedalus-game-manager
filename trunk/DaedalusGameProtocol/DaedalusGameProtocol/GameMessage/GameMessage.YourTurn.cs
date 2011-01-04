﻿/* $Id: GameMessage.YourTurn.cs 819 2011-01-03 01:46:43Z crosis $
 *
 * Description: Signals to the player that the server is waiting for their next
 * move.
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

namespace DaedalusGameProtocol
{
    public abstract partial class GameMessage
    {
        public class YourTurn : GameMessage
        {
            // Construct a new YourTurn message.
            public YourTurn()
            {
                this.data = typeof(GameMessage.YourTurn).Name.ToString() + "{}";
            }

            // Parse a YourTurn message, given in the specified string.
            public YourTurn(string msg)
            {
                if (!GameMessage.IsYourTurn(msg))
                    throw new Exception("This is not a YourTurn message.");

                // Remove all whitespace characters from the message string.
                string msgString = RemoveAllWhiteSpace(msg);

                this.data = msgString;
            }
        }
    }
}