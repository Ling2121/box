namespace Box {
    public interface IWorldBuildProcess<BDType> where BDType : IBuildData{
        void Build(BDType data);
    }
}