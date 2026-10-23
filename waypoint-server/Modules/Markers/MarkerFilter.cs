using HotChocolate.Data.Filters;

namespace waypoint_server.Modules.Markers
{
    public class MarkerFilterType : FilterInputType<MarkerModel>
    {
        protected override void Configure(
            IFilterInputTypeDescriptor<MarkerModel> descriptor)
        {
            descriptor.BindFieldsExplicitly();

            descriptor.Field(m => m.Id);
            descriptor.Field(m => m.Name);
            descriptor.Field(m => m.Latitude);
            descriptor.Field(m => m.Longitude);
        }
    }
}
