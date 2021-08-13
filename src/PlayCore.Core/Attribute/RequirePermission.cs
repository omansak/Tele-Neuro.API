using System;

// TODO
namespace PlayCore.Core.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method)]
    public class RequirePermission : System.Attribute
    {
        public bool Read { get; }
        public bool Insert { get; }
        public bool Update { get; }
        public bool Delete { get; }
        public bool AccessClosed { get; }
        public RequirePermission(params Permission[] permission)
        {
            foreach (var item in permission)
            {
                switch (item)
                {
                    case Permission.Read:
                        Read = true;
                        break;
                    case Permission.Insert:
                        Insert = true;
                        break;
                    case Permission.Update:
                        Update = true;
                        break;
                    case Permission.Delete:
                        Delete = true;
                        break;
                    case Permission.AccessClosed:
                        AccessClosed = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

    }
    public enum Permission
    {
        Read,
        Insert,
        Update,
        Delete,
        AccessClosed
    }
}
