//
// Revised backup code
// Version 1.2.1 of 24-June-2013
//
## Derived from gplex.frame version of 2-September-2006.
## Code page support for files without a BOM. 
## Left and Right Anchored state support.
## Start condition stack. Two generic params.
## Using fixed length context handling for right anchors
//
##-->defines

using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

##-->version295
##-->usingDcl
{   
    /// <summary>
    /// Summary Canonical example of GPLEX automaton
    /// </summary>
    
    // If the compiler can't find the scanner base class maybe you
    // need to run GPPG with the /gplex option, or GPLEX with /noparser
##-->translate $public sealed partial class $Scanner : $ScanBase
    {
        ScanBuff buffer;
        int currentScOrd;  // start condition ordinal
        
        /// <summary>
        /// The input buffer for this scanner.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ScanBuff Buffer => buffer;

##-->consts

#region user code
##-->codeIncl
#endregion user code

        int state;
        int currentStart = startState[0];
        int code;      // last code read
        int cCol;      // column number of code
        int lNum;      // current line number
        //
        // The following instance variables are used, among other
        // things, for constructing the LLoc location objects.
        //
        int tokPos;        // buffer position at start of token
        int tokCol;        // zero-based column number at start of token
        int tokLin;        // line number at start of token
        int tokEPos;       // buffer position at end of token
        int tokECol;       // column number at end of token
        int tokELin;       // line number at end of token
        string? tokTxt;     // lazily constructed text of token

##-->tableDef

        // ==============================================================
        // == Nested struct used for backup in automata that do backup ==
        // ==============================================================

        struct Context // class used for automaton backup.
        {
            public int bPos;
            public int rPos; // scanner.readPos saved value
            public int cCol;
            public int lNum; // Need this in case of backup over EOL.
            public int state;
            public int cChr;
        }
        
        Context ctx = new Context();

        // =================== End Nested classes =======================

##-->translate $public $Scanner(TextReader reader)
        {
            this.buffer = ScanBuff.GetBuffer(reader);
            this.lNum = 0;
            this.code = '\n'; // to initialize yyline, yycol and lineStart
            GetCode();
        }

        int readPos;

        void GetCode()
        {
            if (code == '\n')  // This needs to be fixed for other conventions
                               // i.e. [\r\n\205\u2028\u2029]
            { 
                cCol = -1;
                lNum++;
            }

            readPos = buffer.Pos;

            // Now read new codepoint.
            code = buffer.Read();
            if (code > ScanBuff.EndOfFile)
            {
                cCol++;
            }
        }

        void MarkToken()
        {
            tokPos = readPos;
            tokLin = lNum;
            tokCol = cCol;
        }
        
        void MarkEnd()
        {
            tokTxt = null;
            tokEPos = readPos;
            tokELin = lNum;
            tokECol = cCol;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        int Peek()
        {
            int rslt, codeSv = code, cColSv = cCol, lNumSv = lNum, bPosSv = buffer.Pos;
            GetCode(); rslt = code;
            lNum = lNumSv; cCol = cColSv; code = codeSv; buffer.Pos = bPosSv;
            return rslt;
        }
        
        // ======== AbstractScanner<> Implementation =========

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "yylex")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yylex")]
        public override int yylex() =>
            Scan();

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        int yypos { get { return tokPos; } }
        
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        int yyline { get { return tokLin; } }
        
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        int yycol { get { return tokCol; } }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "yytext")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yytext")]
        public string yytext
        {
            get 
            {
                if (tokTxt == null) 
                    tokTxt = buffer.GetString(tokPos, tokEPos);
                return tokTxt;
            }
        }

        /// <summary>
        /// Discards all but the first "n" codepoints in the recognized pattern.
        /// Resets the buffer position so that only n codepoints have been consumed;
        /// yytext is also re-evaluated. 
        /// </summary>
        /// <param name="n">The number of codepoints to consume</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        void yyless(int n)
        {
            buffer.Pos = tokPos;
            // Must read at least one char, so set before start.
            cCol = tokCol - 1; 
            GetCode();
            // Now ensure that line counting is correct.
            lNum = tokLin;
            // And count the rest of the text.
            for (int i = 0; i < n; i++) GetCode();
            MarkEnd();
        }
       
        //
        //  It would be nice to count backward in the text
        //  but it does not seem possible to re-establish
        //  the correct column counts except by going forward.
        //
        /// <summary>
        /// Removes the last "n" code points from the pattern.
        /// </summary>
        /// <param name="n">The number to remove</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        void _yytrunc(int n) { yyless(yyleng - n); }
        
        //
        // This is painful, but we no longer count
        // codepoints.  For the overwhelming majority 
        // of cases the single line code is fast, for
        // the others, well, at least it is all in the
        // buffer so no files are touched. Note that we
        // can't use (tokEPos - tokPos) because of the
        // possibility of surrogate pairs in the token.
        //
        /// <summary>
        /// The length of the pattern in codepoints (not the same as 
        /// string-length if the pattern contains any surrogate pairs).
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "yyleng")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yyleng")]
        public int yyleng
        {
            get {
                if (tokELin == tokLin)
                    return tokECol - tokCol;
                else
                    return tokEPos - tokPos;
            }
        }
        
        // ============ methods available in actions ==============

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal int YY_START {
            get { return currentScOrd; }
            set { currentScOrd = value; 
                  currentStart = startState[value]; 
            } 
        }
        
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal void BEGIN(int next) {
            currentScOrd = next;
            currentStart = startState[next];
        }

        // ============== The main tokenizer code =================

        int Scan()
        {
##-->prolog 
                for (; ; )
                {
                    int next;              // next state to enter
                    state = currentStart;
                    while ((next = NextState()) == goStart)
                        // At this point, the current character has no
                        // transition from the current state.  We discard 
                        // the "no-match" char.   In traditional LEX such 
                        // characters are echoed to the console.
                        GetCode();
                    // At last, a valid transition ...    
                    MarkToken();
                    state = next;
                    GetCode();
                    
                    bool contextSaved = false;
                    while ((next = NextState()) > eofNum) { // Exit for goStart AND for eofNum
                        if (state <= maxAccept && next > maxAccept) { // need to prepare backup data
                            // Store data for the *latest* accept state that was found.
                            SaveStateAndPos( ref ctx );
                            contextSaved = true;
                        }
                        state = next;
                        GetCode();
                    }
                    if (state > maxAccept && contextSaved)
                        RestoreStateAndPos( ref ctx );
                    if (state <= maxAccept) 
                    {
                        MarkEnd();
##-->actionCases
                    }
                }
##-->epilog
        }

        void SaveStateAndPos(ref Context ctx)
        {
            ctx.bPos  = buffer.Pos;
            ctx.rPos  = readPos;
            ctx.cCol  = cCol;
            ctx.lNum  = lNum;
            ctx.state = state;
            ctx.cChr  = code;
        }

        void RestoreStateAndPos(ref Context ctx)
        {
            buffer.Pos = ctx.bPos;
            readPos = ctx.rPos;
            cCol  = ctx.cCol;
            lNum  = ctx.lNum;
            state = ctx.state;
            code  = ctx.cChr;
        }


        // ============= End of the tokenizer code ================

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal void ECHO() { Console.Out.Write(yytext); }
        
##-->userCode
##-->translate    } // end class $Scanner
} // end namespace
