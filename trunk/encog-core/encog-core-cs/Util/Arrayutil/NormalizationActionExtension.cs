// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
// 5/5/11 3:33 PM    
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
namespace Encog.Util.Arrayutil
{
    public static class NormalizationActionExtension
    {
        /// <returns>True, if this is a classify.</returns>
        public static bool IsClassify(this NormalizationAction extensionParam)
        {
            return (extensionParam == NormalizationAction.OneOf) || (extensionParam == NormalizationAction.SingleField)
                   || (extensionParam == NormalizationAction.Equilateral);
        }
    }
}