# Media Foundation .NET

This library is a cvs import from a project on SourceForge by snarfle.

The original version of this library is available at http://mfnet.SourceForge.Net and is the result of work done by:
snarfle@users.SourceForge.net

In addition, the repo also contains an adapter/wrapper project - SuperMFLib.  More details of this project can be found in its local [readme](SuperMFLib/README.md)


##Original readme

## See WhatsNew.txt for a list of what has changed since the previous version.
Purpose of this library - What it is, what it isn't
Despite the fact that Microsoft has just released a brand new interface for working with multimedia in Vista, they haven’t chosen to provide access to it for managed code developers.  This library opens up MF for use in managed applications.
Reviewing the source code will show that there is very little executable code in this library.  There are a few helper functions, but everything else in the library is just definitions.  There isn’t some large complex set of classes hidden somewhere that does the work.  Essentially, this library is a simple mapping between managed and unmanaged code.  As a result, there is little opportunity for any problems you encounter to be caused by the library.  Not zero chance, but by and large, you may expect that problems you encounter when using the library are not a result of bugs in the library (see “Reporting bugs” below).
Although there are over 100 interfaces defined in the source code, only some of them have been tested to ensure that they will work correctly (see “Using untested interfaces” and “The plan for additional interfaces” below).  The list of interfaces indicating which ones have been tested can be found at interaces.txt.
In addition to the library, a number of samples are provided showing how to use the library.  The samples are in a separate download.

## Licensing
This library is licensed under GNU LESSER GENERAL PUBLIC LICENSE, Version 2.1.  See license.txt for the complete text.  I’m a programmer, not a lawyer, so please, if you have questions regarding licensing, direct them to your lawyer (who undoubtedly knows more about LGPL than I do).

## Supported platforms, languages
This library is usable from c#, and should work from other .NET compatible languages.  It was tested against Vista sp0 using the 2.0 .NET framework.  The library should work for both 32 bit and 64 bit versions of both Vista.
I have also used this in 64bit versions of Windows 7.

## Where to find the documentation
Since the purpose of this library is to allow for the calling of the Media Foundation interfaces, the primary source of documentation is MSDN.  You can find the Media Foundation documentation at http://msdn2.microsoft.com/en-us/library/ms694197.aspx (at least until the next time Microsoft re-orgs their web site).  The few helper classes in the library are documented below in “Helper Classes”.

## Differences between MSDN docs and using this library
It most cases, you should expect that you call the interface methods the same way MSDN says it should be called.  The primary differences are places where MSDN documents method parameters as int, but then says the content of the int is some enum.  The library’s definition is changed to be the enum, so Visual Studio’s intelli-sense will offer up the legal values while typing code.
The names of the structures and enum have been changed to use a more .NET friendly style: AM_SAMPLE2_PROPERTIES -> AMSample2Properties.  Note that the original C++ structure or enum name is provided in an attribute above the declaration.  If there is some specific declaration you need to find, do a scan of the library’s source files for the name.
Also see the sections below on “Error handling” and “How and when to use Marshal.ReleaseComObject()”.

## Error handling
(If you are already familiar with PreserveSig and the effects it has on COM interfaces, you can skip this section.)

How and when to use Marshal.ReleaseComObject()
.NET has a built-in limitation when handling COM methods.  This problem affects any .NET class calling COM, not just this library.  The problem description is rather involved, but boils down to two key facts:
1) Objects in .NET aren't released as soon as they go out of scope.  You have to wait for the Garbage Collector to free them.  As a result, you may get "in use" errors for items you thought should be released.  GC.Collect may help with this.  Also, you can use Marshal.ReleaseComObject.  However, that may introduce other problems (see #2).
2) If you create a COM object, then do 
IGraphBuilder  graphBuilder = (IGraphBuilder) new FilterGraph();
IMediaControl  imc = graphBuilder as IMediaControl;
Calling Marshal.ReleaseComObject on either graphBuilder or imc will invalidate BOTH graphBuilder and imc.  For a good article on this, read Chris Brumme's weblog at http://blogs.msdn.com/cbrumme/archive/2003/04/16/51355.aspx.

## Helper Classes
While the library primarily provides mappings to the Media Foundation interfaces, there are a few helper functions defined that may be useful:

### PropVariant
This class is a c# implementation of the PROPVARIANT structure.  It has a collection of constructors to allow creation of various forms of PROPVARIANTs, and accessors to allow reading.  
Note that since this is implemented as a class, there is no need to have any c# equivalent to VariantInit, and since custom marshaling is used in parameter passing, there is no need to release variants between calls to avoid leaks.  However, there is a Clear() method to ensure timely release of data should that be desirable.

### ConstPropVariant
This class is the parent of PropVariant.  It is used for parameters that are sent from unmanaged code where the callee (ie the managed code) should NOT clear the variant.

### MFInt
This class is a wrapper for ints.  The only time you should need to use this class is if you are dealing with parameters to (or from) MF that are “out int”, but “can be null.”  If you are implementing a method that has such a datatype, you will want to check the value to see if it is null, then use the Assign() method to assign a value.  See GetStreamLimits in the MFT_GrayScale sample for how this works.

### FourCC
This class is useful for processing 4cc (see http://msdn2.microsoft.com/en-us/library/ms783788.aspx for a discussion of some of the basics of 4 character codes).  It contains methods for converting from ints to 4cc’s and Guids.

### COMBase
This class is intended to be the parent of classes that implement COM interfaces.  It can also be used as a parent to classes that merely call COM classes, and the methods can also be called statically.  Note: There is no requirement to use this class, but it can be helpful.

Succeeded: Checks to see if an HRESULT code is not a failure code (ie >= 0).  This isn’t generally useful since most methods in the library with throw exceptions rather than returning HRESULTs.
Failed: Checks to see if an HRESULT code is a failure code (ie < 0).  This isn’t generally useful since most methods in the library with throw exceptions rather than returning HRESULTs.
SafeRelease: Calls Dispose or Marshal.ReleaseComObject against objects.  Checks for null.

### MFError
This class contains two methods.  
ThrowExceptionForHR is a wrapper for Marshal.ThrowExceptionForHR, but additionally provides descriptions for any Media Foundation specific error messages.  Note that you do not have to check for negative values before calling this method.  If the hr value is not a fatal error, no exception will be thrown:

    hr = iSomething.DoSomething();
    MFError.ThrowExceptionForHR(hr);

GetErrorText returns a string representation of HRESULTs.  This works for both Media Foundation error codes, and general COM error codes.

### WaveFormatEx, WaveFormatExWithData, WaveFormatExtensible, WaveFormatExtensibleWithData
C++ has this trick they do where they pass a pointer to a WaveFormatEx, then you parse the data to figure out whether the struct is really a WaveFormatEx, or a WaveFormatExtensible.  What’s more, there can be some random number of bytes *after* the structure.  In order to deal with this in c# (which doesn’t care for these types of shenanigans), these 4 items have been declared as classes with methods to facilitate performing this same magic for you automatically in c#.  
If you call a method that returns a “WaveFormatEx,”  you can determine if it is a WaveFormatExtensible by using the c# as operator to cast it (among other methods).

### BitmapInfoHeader, BitmapInfoHeaderWithData
BitmapInfoHeader has the same issues as WaveFormatEx.  See the description above.

### PVMarshaler, RTIMarshaler, RTAMarshaler, GAMarshaler, WEMarshaler, BMMarshaler
These classes handle the custom marshaling for PropVariants, WaveFormatEx, MFTGetInfo, MFTRegister, MFTEnum.  There should be no reason to use any of these directly.

## Using untested interfaces
The MediaFoundation source code contains interfaces for virtually all of Media Foundation as of the date it was created.  Currently, however, only some of them have been tested.  Testing includes performing each of these on each method of the interface:
1) Checking to see that the method is defined correctly according to the MSDN docs
2) Change int to enum where needed
3) Add any needed MarshalAs
4) Verify that any needed structs are correctly defined 
5) Remove unneeded "ref", add needed "out"
6) (Where possible) remove IntPtr and use actual structs or classes
7) Where necessary, create custom marshalers for passing structs
8) Write code that calls the method, making sure parameters that are documented to accept null, do so
However, it may be that you need to use an interface that hasn't been tested yet.  All the untested interfaces, structs and enum are included in the source, but are wrapped within:


    #if ALLOW_UNTESTED_INTERFACES
    #endif


Release builds of the library do not expose these interfaces.  If you want to enable all the methods, you can use #define in specific source files or globally with project properties to define this constant, and then build your own copy of the library.  However, don't be surprised if some of the methods/interfaces that haven't been tested don't work correctly.  Also, when the method is eventually tested, the definition may be modified.

## The plan for additional interfaces
The plan is to continue testing interfaces and moving them out of the #if.  However, no particular schedule is planned at this point.  If you have some specific methods that you would like to see tested, or if you have tested (all the methods on) an interface and would like to see it included in the library, post a message to the Open Discussion forum discussed in “Links for help” below.

## Reporting bugs
Bugs in this library should be reported to http://sourceforge.net/tracker/?group_id=199925&atid=971446.
Note that only bugs in the library, samples, or docs that are part of this project should be reported here.  Bugs in Media Foundation itself must be reported to Microsoft.

## Links for help
Your best place to start is the MSDN docs (see links under "Where to find the documentation").  If your question is about Media Foundation in general, you might want to post to one of Microsoft's newsgroups (currently at http://social.msdn.microsoft.com/forums/en-US/mediafoundationdevelopment/threads/).  If your question is specific to this library, you should post your question post in the “Open Discussion” forum here: http://sourceforge.net/forum/forum.php?forum_id=711229.

## Errata
The following items are minor variations between the MSDN docs and the .Net implementation.  I don’t expect these to ever be an issue for anyone, but I wanted a place to keep track of them.
Output parameters that “can be NULL if the value is not needed” present problems for .Net.  If you declare them as “out,” then if someone tries to pass you a NULL, the .Net framework rejects the call.  The only way around this is to declare the parameter as IntPtr.  This is a PITA for the majority of people who just want to use the typed field, and will never try to implement an interface.  For the cases below, I don’t believe the interface is intended to be implemented, only called.
There are a few methods on a few interfaces where the return value from the method is NOT an HRESULT.  In these cases, you must use PreserveSig to make sure the actual value can be retrieved.  It’s possible that people will assume that all methods will throw an exception, and not manually throw the exception on the failure.  Doesn’t seem like a big problem.  There are also a few cases where the method might return S_* error codes.  However, the status messages don’t seem to be of value, so I have left the PreserveSig off for them, resulting in any potential S_ code being discarded.

```
IMFSourceResolver::BeginCreateObjectFromURL cookie param can be null
IMFAttributes::GetString p4 can be null
IMFVideoDisplayControl::SetBorderColor uses int instead of COLORREF (this is done a lot)
IMFMediaBuffer::Lock can accept nulls
MFVideoSurfaceInfo has unmarshalled array.  Custom?
IMFMediaTypeHandler::IsMediaTypeSupported has optional parm
```
