using System.ComponentModel;


/* Unmerged change from project 'Gapotchenko.FX.Reflection.Loader (net5.0)'
Before:
namespace System.Runtime.CompilerServices
{
#if !TFF_ISEXTERNALINIT
After:
namespace System.Runtime.CompilerServices;
#if !TFF_ISEXTERNALINIT
*/
namespace System.Runtime.CompilerServices;

#if !TFF_ISEXTERNALINIT
[EditorBrowsable(EditorBrowsableState.Never)]
static class IsExternalInit
{
}
#endif
