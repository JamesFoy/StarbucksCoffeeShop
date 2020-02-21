namespace AssetUsageDetectorNamespace
{
    /// Credit: http://stackoverflow.com/questions/724143/how-do-i-create-a-delegate-for-a-net-property
    public interface IPropertyAccessor
    {
        object GetValue( object source );
    }
}