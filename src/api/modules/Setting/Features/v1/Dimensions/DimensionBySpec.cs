using Ardalis.Specification;
using FSH.Starter.WebApi.Setting.Domain;

namespace FSH.Starter.WebApi.Setting.Features.v1.Dimensions
{
    public sealed class DimensionByIdSpec : Specification<Dimension, DimensionDto>, ISingleResultSpecification<Dimension>
    {
        public DimensionByIdSpec(Guid id) =>
            Query
                .Where(e => e.Id == id);
    }

    public sealed class DimensionByCodeSpec : Specification<Dimension>, ISingleResultSpecification<Dimension>
    {
        public DimensionByCodeSpec(string code) =>
            Query
                .Where(e => e.Code == code);
    }

    public sealed class DimensionByNameSpec : Specification<Dimension>, ISingleResultSpecification<Dimension>
    {
        public DimensionByNameSpec(string name) =>
            Query
                .Where(e => e.Name == name);
    }
    
    public sealed class DimensionByFatherIdSpec : Specification<Dimension>
    {
        public DimensionByFatherIdSpec(Guid id) =>
            Query
                .Where(e => e.FatherId == id);
    }

}
