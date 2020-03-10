using System;

namespace VueExample.Exceptions 
{

    public class CollectionIsEmptyException : Exception 
    {
        public CollectionIsEmptyException () 
        {

        }

        public CollectionIsEmptyException (string message) : base (message) 
        {

        }
    }

}