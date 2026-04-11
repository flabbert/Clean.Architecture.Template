using Clean.Architecture.Template.Domain;
using Facet;

namespace Clean.Architecture.Template.Application.Features.DummyItems;

[Facet(typeof(DummyItem), exclude: [nameof(DummyItem.Id)])]
public partial record CreateDummyItemRequest;

[Facet(typeof(DummyItem))]
public partial record DummyItemResponse;
