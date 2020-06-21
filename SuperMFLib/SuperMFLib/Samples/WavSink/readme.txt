/************************************************************************
WavSink - A COM object that write .wav files and a program to exercise it

While the underlying libraries are covered by LGPL, this sample is released 
as public domain.  It is distributed in the hope that it will be useful, but 
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
or FITNESS FOR A PARTICULAR PURPOSE.  

**************************************************************************/

This sample is a c# version of the WavSink sample included in the
Media Foundation SDK.  Is the opposite of the WavSource sample: It writes 
audio output to a .wav file.

There has been a major change to how error handling is done since this sample
was first released.  Now all COM methods return an int (HRESULT) that must
explicitly be check to make sure the method worked as expected.  Commonly
this would be done as:

   int hr;

   hr = iSomething.DoSomething();
   MFError.ThrowExceptionForHR(hr); // Turn hr into exception if it was an error

After you build this sample, you will need to register it with both COM and MF.  
In theory, you can do this by clicking the "Register as COM" box in Visual Studio.  
In practice, I find that this doesn't work on Vista due to the way they have
mucked with security.  Instead, I use this command line from a cmd window opened 
with "Run as administrator":

c:\Windows\Microsoft.NET\Framework\v2.0.50727\regasm /tlb /codebase WavSink.dll

The way MF loads sinks seems strange (to me).  Look at this comment from the c++ code:

// To use the WavSink, include this header file in an application
// and link to the library file created by this project.

So, unlike DirectShow, there isn't any way to enumerate sinks, and you must statically
link to them to use them.  Whatever.

So, to use the c# version of the wave sink:

If you are calling it from managed code, simply use the assembly, and do a 
"new CWavSink(pStream)".  

If you are calling it from c++, you can use CoCreateInstance to create an 
instance of the class, then call IMFCustom::SetStream to provide the stream.

