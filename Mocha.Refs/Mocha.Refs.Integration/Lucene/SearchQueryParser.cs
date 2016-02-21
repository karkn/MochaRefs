using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Mocha.Refs.Integration.Lucene
{
    public class SearchQueryParser
    {
        private State _state;

        private StringBuilder _buf;

        private List<string> _texts;
        private List<string> _tags;
        private List<string> _users;

        private int _index;
        private string _queryText;

        public IEnumerable<string> Texts
        {
            get { return _texts; }
        }

        public IEnumerable<string> Tags
        {
            get { return _tags; }
        }

        public IEnumerable<string> Users
        {
            get { return _users; }
        }

        /// <summary>
        /// ""を語句として切り出す。
        /// []をタグとして切り出す。
        /// user:ユーザーとして切り出す。
        /// </summary>
        public void Parse(string queryText)
        {
            _state = State.Normal;
            _queryText = queryText;
            _texts = new List<string>();
            _tags = new List<string>();
            _users = new List<string>();
            _buf = new StringBuilder();

            for (_index = 0; _index < queryText.Length; ++_index)
            {
                var ch = queryText[_index];
                Next(ch);
            }

            Finish();
        }

        private void Next(char ch)
        {
            switch (_state)
            {
                case State.Normal:
                    ParseNormal(ch);
                    break;
                case State.Words:
                    ParseQuoted(ch);
                    break;
                case State.Tag:
                    ParseTag(ch);
                    break;
                case State.User:
                    ParseUser(ch);
                    break;
            }

        }

        public void Finish()
        {
            switch (_state)
            {
                case State.Normal:
                case State.Words:
                    var text = _buf.ToString().Trim();
                    _buf.Clear();
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        _texts.Add(text);
                    }
                    break;
                case State.Tag:
                    /// 閉じ]がなかった
                    var tag = _buf.ToString().Trim();
                    _buf.Clear();
                    if (!string.IsNullOrWhiteSpace(tag))
                    {
                        _tags.Add(tag);
                    }
                    break;
                case State.User:
                    var user = _buf.ToString().Trim();
                    _buf.Clear();
                    if (!string.IsNullOrWhiteSpace(user))
                    {
                        _users.Add(user);
                    }
                    break;
            }
        }

        private void ParseNormal(char ch)
        {
            switch (ch)
            {
                case '"':
                    _state = State.Words;
                    break;
                case '[':
                    _state = State.Tag;
                    break;
                case 'u':
                    var look = LookNext(5); /// 5 == "user:".length
                    if (look == "user:")
                    {
                        _state = State.User;
                        Succ(4);
                    }
                    else
                    {
                        _buf.Append(ch);
                    }
                    break;
                case ' ':
                    var text = _buf.ToString().Trim();
                    _buf.Clear();
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        _texts.Add(text);
                    }
                    break;
                default:
                    _buf.Append(ch);
                    break;
            }
        }

        private void ParseQuoted(char ch)
        {
            switch (ch)
            {
                case '"':
                    var text = _buf.ToString().Trim();
                    _buf.Clear();
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        _texts.Add(text);
                    }
                    _state = State.Normal;
                    break;
                default:
                    _buf.Append(ch);
                    break;
            }
        }

        private void ParseTag(char ch)
        {
            switch (ch)
            {
                case ']':
                    var tag = _buf.ToString().Trim();
                    _buf.Clear();
                    if (!string.IsNullOrWhiteSpace(tag))
                    {
                        _tags.Add(tag);
                    }
                    _state = State.Normal;
                    break;
                case '[':
                    /// do nothing
                    break;
                default:
                    _buf.Append(ch);
                    break;
            }
        }

        private void ParseUser(char ch)
        {
            if (IsUserNameChar(ch))
            {
                _buf.Append(ch);
            }
            else
            {
                var user = _buf.ToString().Trim();
                _buf.Clear();
                if (!string.IsNullOrWhiteSpace(user))
                {
                    _users.Add(user);
                }
                _state = State.Normal;
            }
        }

        private bool IsUserNameChar(char ch)
        {
            return char.IsDigit(ch) || char.IsLetter(ch);
        }

        private string LookNext(int count)
        {
            return _index + count <= _queryText.Length ?
                _queryText.Substring(_index, count) :
                _queryText.Substring(_index);
        }

        private void Succ(int count)
        {
            _index += count;
        }

        private enum State
        {
            Normal,
            Words,
            Tag,
            User,
        }
    }
}