namespace Pelican.Mediator
{
    internal struct Void
    {
        public Void() { }

        public static Void Empty => new Void();

        public static Task<Void> Task => System.Threading.Tasks.Task.FromResult(Empty);
    }
}
