using Core.Domain;
using Core.Specifications;

namespace Core.Features.Tags.Specifications;

public class TagsWithIdsSpecification : BaseSpecification<Tag>
{
  public TagsWithIdsSpecification(IEnumerable<int> ids)
      : base(x => ids.Contains(x.Id))
  {
  }
}