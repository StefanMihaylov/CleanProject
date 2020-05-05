using System;
using System.Runtime.InteropServices;
using System.Text;

namespace CleanProject
{
    internal static class NativeMethods
    {
        /// <summary>
        ///     Retrieves a handle to the top-level window whose class name and window name match the specified strings. This
        ///     function does not search child windows. This function does not perform a case-sensitive search.
        ///     <para>
        ///         PInvode map to the
        ///         <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms633586(v=vs.85).aspx">
        ///             <strong xmlns="http://www.w3.org/1999/xhtml">FindWindow</strong>
        ///         </a>
        ///         function
        ///     </para>
        /// </summary>
        /// <param name="sClassName">
        ///     <para>
        ///         The class name or a class atom created by a previous call to the
        ///         <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms633586(v=vs.85).aspx">
        ///             <strong xmlns="http://www.w3.org/1999/xhtml">RegisterClass</strong>
        ///         </a>
        ///         or
        ///         <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms633587(v=vs.85).aspx">
        ///             <strong xmlns="http://www.w3.org/1999/xhtml">RegisterClassEx</strong>
        ///         </a>
        ///         function. The atom must be in the low-order word of lpClassName; the high-order word must be zero.
        ///     </para>
        ///     <para>
        ///         If lpClassName points to a string, it specifies the window class name. The class name can be any name
        ///         registered with RegisterClass or RegisterClassEx, or any of the predefined control-class names.
        ///     </para>
        ///     <para>If lpClassName is NULL, it finds any window whose title matches the lpWindowName parameter.</para>
        /// </param>
        /// <param name="sAppName">
        ///     The window name (the window's title). If this parameter is NULL, all window names match.
        /// </param>
        /// <returns>
        ///     <para>
        ///         If the function succeeds, the return value is a handle to the window that has the specified class name and
        ///         window name.
        ///     </para>
        ///     <para>
        ///         If the function fails, the return value is NULL. To get extended error information, call
        ///         <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms679360(v=vs.85).aspx">
        ///             <strong xmlns="http://www.w3.org/1999/xhtml">GetLastError</strong>
        ///         </a>
        ///         .
        ///     </para>
        /// </returns>
        /// <remarks>
        ///     If the lpWindowName parameter is not NULL, FindWindow calls the
        ///     <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms633520(v=vs.85).aspx">
        ///         <strong xmlns="http://www.w3.org/1999/xhtml">GetWindowText</strong>
        ///     </a>
        ///     function to retrieve the window name for comparison.
        ///     <para>For a description of a potential problem that can arise, see the Remarks for <strong>GetWindowText</strong>.</para>
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string sClassName, string sAppName);

        /// <summary>
        ///     Converts the specified path to its long form.
        ///     <para>
        ///         PInvode map to the
        ///         <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/aa364980(v=vs.85).aspx">
        ///             <strong xmlns="http://www.w3.org/1999/xhtml">GetLongPathName</strong>
        ///         </a>
        ///         function
        ///     </para>
        /// </summary>
        /// <param name="path">
        ///     The path to be converted.
        /// </param>
        /// <param name="pszPath">
        ///     A pointer to the buffer to receive the long path.
        /// </param>
        /// <param name="cchPath">
        ///     The size of the buffer lpszLongPath points to, in TCHARs.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is the length, in TCHARs, of the string copied to lpszLongPath, not
        ///     including the terminating null character.
        ///     <para>
        ///         If the lpBuffer buffer is too small to contain the path, the return value is the size, in TCHARs, of the
        ///         buffer that is required to hold the path and the terminating null character.
        ///     </para>
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetLongPathName(string path, StringBuilder pszPath, int cchPath);

        /// <summary>Shows a Window</summary>
        /// <remarks>
        ///     <para>
        ///         To perform certain special effects when showing or hiding a
        ///         window, use AnimateWindow.
        ///     </para>
        ///     <para>
        ///         The first time an application calls ShowWindow, it should use
        ///         the WinMain function's nCmdShow parameter as its nCmdShow parameter.
        ///         Subsequent calls to ShowWindow must use one of the values in the
        ///         given list, instead of the one specified by the WinMain function's
        ///         nCmdShow parameter.
        ///     </para>
        ///     <para>
        ///         As noted in the discussion of the nCmdShow parameter, the
        ///         nCmdShow value is ignored in the first call to ShowWindow if the
        ///         program that launched the application specifies startup information
        ///         in the structure. In this case, ShowWindow uses the information
        ///         specified in the STARTUPINFO structure to show the window. On
        ///         subsequent calls, the application must call ShowWindow with nCmdShow
        ///         set to SW_SHOWDEFAULT to use the startup information provided by the
        ///         program that launched the application. This behavior is designed for
        ///         the following situations:
        ///     </para>
        ///     <list type="">
        ///         <item>
        ///             Applications create their main window by calling CreateWindow
        ///             with the WS_VISIBLE flag set.
        ///         </item>
        ///         <item>
        ///             Applications create their main window by calling CreateWindow
        ///             with the WS_VISIBLE flag cleared, and later call ShowWindow with the
        ///             SW_SHOW flag set to make it visible.
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <param name="hWnd">Handle to the window.</param>
        /// <param name="nCmdShow">
        ///     Specifies how the window is to be shown.
        ///     This parameter is ignored the first time an application calls
        ///     ShowWindow, if the program that launched the application provides a
        ///     STARTUPINFO structure. Otherwise, the first time ShowWindow is called,
        ///     the value should be the value obtained by the WinMain function in its
        ///     nCmdShow parameter. In subsequent calls, this parameter can be one of
        ///     the WindowShowStyle members.
        /// </param>
        /// <returns>
        ///     If the window was previously visible, the return value is nonzero.
        ///     If the window was previously hidden, the return value is zero.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}