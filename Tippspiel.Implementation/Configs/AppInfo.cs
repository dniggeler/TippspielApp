using System.Collections.Generic;

namespace Tippspiel.Implementation
{
    public class AppInfo<TSubclass> where TSubclass : AppInfo<TSubclass>, new()
    {
        private static Dictionary<string, TSubclass> _memoryStorage = null;

        static AppInfo()
        {
            _memoryStorage = new Dictionary<string, TSubclass>();

            _memoryStorage.Add(Key, null);
        }

        private static string Key
        {
            get { return typeof(AppInfo<TSubclass>).FullName; }
        }

        private static TSubclass Value
        {
            get
            {
                return _memoryStorage[Key];
            }

            set
            {
                _memoryStorage[Key] = value;
            }
        }

        public static TSubclass Current
        {
            get
            {
                var instance = Value;
                if (instance == null)
                    lock (typeof(TSubclass)) // not ideal to lock on a type -- but it'll work
                    {
                        // standard lock double-check
                        instance = Value;
                        if (instance == null)
                        {
                            Value = instance = new TSubclass();
                        }
                    }
                return instance;
            }
        }
    }

}
