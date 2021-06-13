using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Scanner
{
    class Scanner
    {
        private string source;
        private int start = 0;
        private int current = 0;
        private int line = 1;
        public Action<int, string> Error;

        public List<Token> Tokens
        {
            get;
            private set;
        } = new List<Token>();

        private bool EOF
        {
            get
            {
                return current >= source.Length;
            }
        }

        private char Next
        {
            get
            {
                return source[current++];
            }
        }

        private char Peek
        {
            get
            {
                if (EOF)
                {
                    return '\0';
                }

                return source[current];
            }
        }

        private char PeekNext
        {
            get
            {
                if (current + 1 >= source.Length)
                {
                    return '\0';
                }

                return source[current + 1];
            }
        }

        public Scanner(string source)
        {
            this.source = source;
        }

        public void ScanTokens()
        {
            while(!EOF)
            {
                start = current;

                ScanToken();
            }

            Tokens.Add(new Token(TokenType.EOF, "", null, line));
        }

        private void ScanToken()
        {
            char c = Next;

            switch(c)
            {
                case '(':
                    
                    AddToken(TokenType.LeftParenthesis);
                    
                    break;

                case ')':
                    
                    AddToken(TokenType.RightParenthesis);
                    
                    break;

                case '{':
                    
                    AddToken(TokenType.LeftBrace);
                    
                    break;

                case '}':
                    
                    AddToken(TokenType.RightBrace);
                    
                    break;

                case ',':
                    
                    AddToken(TokenType.Comma);
                    
                    break;

                case '.':
                    
                    AddToken(TokenType.Dot);
                    
                    break;

                case '-':
                    
                    AddToken(TokenType.Minus);
                    
                    break;

                case '+':
                    
                    AddToken(TokenType.Plus);
                    
                    break;

                case ';':
                    
                    AddToken(TokenType.Semicolon);
                    
                    break;

                case '*':

                    AddToken(TokenType.Star);

                    break;

                case '!':

                    AddToken(Matches('=') ? TokenType.BangEqual : TokenType.Bang);

                    break;

                case '=':

                    AddToken(Matches('=') ? TokenType.EqualEqual : TokenType.Equal);

                    break;

                case '<':

                    AddToken(Matches('=') ? TokenType.LessEqual : TokenType.Less);

                    break;

                case '>':

                    AddToken(Matches('=') ? TokenType.GreaterEqual : TokenType.Greater);

                    break;

                case '/':

                    if (Matches('/'))
                    {
                        // A comment goes until the end of the line.
                        while (Peek != '\n' && !EOF)
                        {
                            _ = Next;
                        }
                    }
                    else
                    {
                        AddToken(TokenType.Slash);
                    }

                    break;

                case ' ':
                case '\r':
                case '\t':

                    // Ignore whitespace.
                    break;

                case '\n':

                    line++;

                    break;

                case '"':
                    
                    ScanString();
                    
                    break;

                case 'o':

                    if(Matches('r'))
                    {
                        AddToken(TokenType.Or);
                    }

                    break;

                default:

                    if(char.IsDigit(c))
                    {
                        HandleNumber();
                    }
                    else if(IsAlpha(c))
                    {
                        HandleIdentifier();
                    }
                    else
                    {
                        Error(line, "Unexpected character");
                    }

                    break;
            }
        }

        private void HandleIdentifier()
        {
            while(IsAlpha(Peek))
            {
                _ = Next;
            }

            var text = source.Substring(start, current - start);

            var tokenType = Keywords.Keyword(text);

            if(tokenType.HasValue)
            {
                AddToken(tokenType.Value);
            }
            else
            {
                AddToken(TokenType.Identifier);
            }
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z') ||
                c == '_';
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || char.IsDigit(c);
        }

        private void HandleNumber()
        {
            while (char.IsDigit(Peek))
            {
                _ = Next;
            }

            // Look for a fractional part.
            if (Peek == '.' && char.IsDigit(PeekNext))
            {
                // Consume the "."
                _ = Next;

                while (char.IsDigit(Peek))
                {
                    _ = Next;
                }
            }

            if(double.TryParse(source.Substring(start, current - start), out var value))
            {
                AddToken(TokenType.Number, value);
            }
            else
            {
                Error(line, "Invalid number");
            }
        }

        private void ScanString ()
        {
            while (Peek != '"' && !EOF)
            {
                if (Peek == '\n')
                {
                    line++;
                }

                _ = Next;
            }

            if (EOF)
            {
                Error(line, "Unterminated string.");

                return;
            }

            // The closing ".
            _ = Next;

            // Trim the surrounding quotes.
            var value = source.Substring(start + 1, current - start - 2);

            AddToken(TokenType.String, value);
        }

        private bool Matches(char expected)
        {
            if(EOF || source[current] != expected)
            {
                return false;
            }

            current++;

            return true;
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object literal)
        {
            string text = source.Substring(start, current - start);

            Tokens.Add(new Token(type, text, literal, line));
        }
    }
}
