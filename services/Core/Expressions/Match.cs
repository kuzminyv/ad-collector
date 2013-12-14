using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Utils;

namespace Core.Expressions
{
    public class Match : IEnumerable, IEnumerable<Match>
    {
        private readonly string _name;
        public string Name
        {
            get
            {
                return _name;
            }
        }

        private readonly string _value;
        public string Value
        {
            get
            {
                return _value;
            }
        }

        public string Path
        {
            get
            {
                Match parent = Parent;
                string path = Name;
                while (parent != null)
                {
                    path = parent.Name + "\\" + path;
                    parent = parent.Parent;
                }

                return path;
            }
        }

        private Match _parent;
        public Match Parent
        {
            get
            {
                return _parent;
            }
        }

        public IEnumerable<Match> Matches
        {
            get 
            {
                return _matches;
            }
        }

        private readonly List<Match> _matches;

        /// <summary>
        /// Returns value of first match with specified name. (For backward capability all html tags cutted from result.)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string this[string name] 
        {
            get
            {
                var match = _matches.FirstOrDefault(m => m.Name == name);
                if (match != null)
                {
                    return string.IsNullOrEmpty(match.Value) ? match.Value : match.Value.HTMLToText();
                }
                return null;
            }
        }

        public IEnumerable<Match> GetByPath(string path, bool relative)
        {
            foreach (Match match in Match.Flat(this))
            {
                if ((relative && match.GetRelativePath(this).StartsWith(path)) || (!relative && match.Path.StartsWith(path)))
                {
                    yield return match;
                }
            }
        }

        public string GetRelativePath(Match match)
        {
            Match parent = Parent;
            string path = Name;
            while (parent != null && parent != match)
            {
                path = parent.Name + "\\" + path;
                parent = parent.Parent;
            }

            return path;
        }

        public Match AddMatch(Match match)
        {
            _matches.Add(match);
            match._parent = this;
            return match;
        }

        public void AddMatchRange(IEnumerable<Match> range)
        {
            foreach (var match in range)
            {
                AddMatch(match);
            }
        }

        public IEnumerator<Match> GetEnumerator()
        {
            return _matches.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _matches.GetEnumerator();
        }

        public Match(string name, string value, params Match[] matches)
        {
            _name = name;
            _value = value;
            _matches = new List<Match>();
            foreach (Match m in matches)
            {
                AddMatch(m);
            }
        }

        /// <summary>
        /// Returns tree of matches as plain list
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public static IEnumerable<Match> Flat(Match match)
        {
            if (match.Count() > 0)
            {
                foreach (var child in match)
                {
                    foreach (var m in Flat(child))
                    {
                        yield return m;
                    }
                }
            }
            else
            {
                yield return match;
            }
        }

        public override string ToString()
        {
            StringBuilder stringBulder = new StringBuilder();

            foreach (var match in Match.Flat(this))
            {
                stringBulder.AppendFormat("{0} : {1};", match.Name, match.Value);
            }
            return stringBulder.ToString();
        }
    }
}
