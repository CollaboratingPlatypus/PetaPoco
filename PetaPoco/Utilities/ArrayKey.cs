// <copyright file="ArrayKey.cs" company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/05</date>

namespace PetaPoco.Internal
{
    internal class ArrayKey<T>
    {
        private int _hashCode;

        private T[] _keys;

        public ArrayKey(T[] keys)
        {
            // Store the keys
            _keys = keys;

            // Calculate the hashcode
            _hashCode = 17;
            foreach (var k in keys)
            {
                _hashCode = _hashCode*23 + (k == null ? 0 : k.GetHashCode());
            }
        }

        private bool Equals(ArrayKey<T> other)
        {
            if (other == null)
                return false;

            if (other._hashCode != _hashCode)
                return false;

            if (other._keys.Length != _keys.Length)
                return false;

            for (int i = 0; i < _keys.Length; i++)
            {
                if (!object.Equals(_keys[i], other._keys[i]))
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ArrayKey<T>);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}