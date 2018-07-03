namespace PanoramaUITools.Panorama
{
    internal struct PanoramaDllParams
    {
        public byte[] ResourceReference { get; }
        public sbyte ResourceReferenceOffset { get; }
        public byte SignatureCheckJumpInstr { get; }

        public PanoramaDllParams(byte[] resourceRef, sbyte resourceRefOffset, byte sigCheckJumpInstr)
        {
            ResourceReference = resourceRef;
            ResourceReferenceOffset = resourceRefOffset;
            SignatureCheckJumpInstr = sigCheckJumpInstr;
        }
    }
}
