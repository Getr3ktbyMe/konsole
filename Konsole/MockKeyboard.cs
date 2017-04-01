﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Konsole
{

    public static class ConsoleKeyInfoExtensions
    {
        public static ConsoleKeyInfo ToKeypress(this char key)
        {
            return new ConsoleKeyInfo(key, (ConsoleKey)key, false, false, false);
        }
    }

    public class MockKeyboard : IReadKey
    {
        private readonly Queue<ConsoleKeyInfo> _keypresses = new Queue<ConsoleKeyInfo>();

        public MockKeyboard(params ConsoleKeyInfo[] keys)
        {
            // not threadsafe, but this is for testing so good enough.
            foreach (var key in keys) _keypresses.Enqueue(key);
        }

        public MockKeyboard(params char[] keys)
        {
            // not threadsafe, but this is for testing so good enough.
            foreach (var key in keys) _keypresses.Enqueue(key.ToKeypress());
        }

        public MockKeyboard()
        {

        }

        public MockKeyboard PressKey(char c, bool shift = false, bool alt = false, bool control = false)
        {
            var keyPress = new ConsoleKeyInfo(c, (ConsoleKey) c, shift, alt, control);
            _keypresses.Enqueue(keyPress);
            return this;
        }

        /// <summary>
        /// pause between keypresses. Expected use for this might be automated demos if you want to simulate user typing on a screen, or integration testing where timing is critical.
        /// </summary>
        public int PauseBetweenKeypresses { get; set; } = 0;

        // can add in a randomiser for making the pauses seem a bit more human.
        // or possilby add the the timing to each PressKey so that we can do a record and replay and simulate real user behavior.
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">if called, and you have not queued up enough keystrokes to process the requests.</exception>
        public ConsoleKeyInfo ReadKey()
        {
            if (PauseBetweenKeypresses != 0) Thread.Sleep(PauseBetweenKeypresses);
            if (_keypresses.Count == 0)
            {
                if (AutoReplyKey == null) throw new InvalidOperationException("MockKeyboard has run out of queued keys to return. Enable auto-reply or queue up more keystrokes.");
                return AutoReplyKey.Value;
            }
            var k = _keypresses.Dequeue();
            return k;
        }

        public ConsoleKeyInfo? AutoReplyKey { get; set; } = null;
    }
}
