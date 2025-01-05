namespace YAMDCC.ECInspector
{
    internal struct ECValue
    {
        /// <summary>
        /// The EC value itself.
        /// </summary>
        public int Value;

        /// <summary>
        /// How long it's been since <see cref="Value"/> was last updated.
        /// </summary>
        public int Age;
    }
}
