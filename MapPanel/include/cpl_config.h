/* $Id: cpl_config.h.wince 11479 2007-05-11 07:25:33Z mloskot $/

/*
 * cpl_config.h with definitions for Windows CE platform.
 */
#ifndef _WIN32_WCE
# error This version of cpl_config.h header is dedicated for Windows CE platform!
#endif

/* Define if you don't have vprintf but do have _doprnt.  */
#undef HAVE_DOPRNT

/* Define if you have the snprintf function.  */
#define HAVE_SNPRINTF
#define snprintf _snprintf

/* Define if you have the vprintf function.  */
#define HAVE_VPRINTF
#define HAVE_VSNPRINTF
#define vsnprintf _vsnprintf

/* Define if you have the `copysign' function. */
#define HAVE_COPYSIGN
#define copysign _copysign
#define copysignf _copysign
#define copysignl _copysign

/* Windows CE does not support getcwd function. */
#undef HAVE_GETCWD
/* #define getcwd _getcwd */

/* Define if you have the stdicmp function.  */
#define HAVE_STRICMP
#define stricmp _stricmp

/* Define if you have the strnicmp function.  */
#define HAVE_STRNICMP
#define strnicmp _strnicmp

/* Define if you have the strdup function.  */
#define HAVE_STRDUP
#define strdup _strdup

/* Define if you have the strerror function.  */
#define HAVE_STRERROR
#define strerror wceex_strerror /* defined in cpl_wince.h */

/* Define if you have the rewind function.  */
#define HAVE_REWIND
#define rewind wceex_rewind /* defined in cpl_wince.h */

/* Define if you have the stat function.  */
#define HAVE_STAT
#define stat wceex_stat

/* Define if you have the unlink function.  */
#define HAVE_UNLINK
#define unlink wceex_unlink

/* Define if you have the mkdir function.  */
#define HAVE_MKDIR
#define mkdir wceex_mkdir

/* Define if you have the rmdir function.  */
#define HAVE_RMDIR
#define rmdir wceex_rmdir

/* Define if you have the rename function.  */
#define HAVE_RENAME
#define rename wceex_rename

/* Define if you have the abort function.  */
#define HAVE_ABORT
#define abort wceex_abort

/* Define if you have the _findfirst function.  */
#define HAVE_FINDFIRST
#define _findfirst wceex_findfirst

/* Define if you have the _findnext function.  */
#define HAVE_FINDNEXT
#define _findnext wceex_findnext

/* Define if you have the _findclose function.  */
#define HAVE_FINDNEXT
#define _findclose wceex_findclose

/* Define if you have the time function.  */
#define HAVE_TIME
#define time wceex_time /* XXX - mloskot */

/* Define if you have the time function.  */
#define HAVE_GMTIME
#define gmtime wceex_gmtime

/* Define if you have the localtime function.  */
#define HAVE_LOCALTIME
#define localtime wceex_localtime

/* Define if you have the ctime function.  */
#define HAVE_CTIME
#define ctime wceex_ctime

/* Define if you have the ctime function.  */
/* wceex_setlocale provides ONLY dummy implementation. */
#define HAVE_SETLOCALE
#define setlocale wceex_setlocale

/* Define to 1 if you have the `bsearch' function. */
#define HAVE_BSEARCH
#define bsearch wceex_bsearch

/* Define to 1 if you have the `lfind' function. */
#define HAVE_LFIND
#define lfind wceex_lfind

/* Define if you have the ANSI C header files.  */
#ifndef STDC_HEADERS
#  define STDC_HEADERS
#endif

/* Windows CE is not have errno.h file: */
#if defined(_WIN32_WCE) && !defined(NO_ERRNO_H)
#  undef HAVE_ERRNO_H
#endif

/* Define to 1 if you have the <search.h> header file. */
#define HAVE_SEARCH_H 1

/* Define to 1 if you have the <fcntl.h> header file. */
#define  HAVE_FCNTL_H 1

/* Define if you have the <unistd.h> header file.  */
#undef HAVE_UNISTD_H

/* Define if you have the <stdint.h> header file.  */
#undef HAVE_STDINT_H

/* Define if you have the <time.h> header file.  */
#undef HAVE_TIME_H

/* Define if you have the <sql.h> header file. */
#undef HAVE_SQL_H

/* Define if you have the <sqlext.h> header file. */
#undef HAVE_SQLEXT_H

#undef HAVE_LIBDL 

#undef HAVE_DLFCN_H
#undef HAVE_DBMALLOC_H
#undef HAVE_LIBDBMALLOC
#undef WORDS_BIGENDIAN

/* The size of a `int', as computed by sizeof. */
#define SIZEOF_INT 4

/* The size of a `long', as computed by sizeof. */
#define SIZEOF_LONG 4

/* #define CPL_DISABLE_DLL */
/* #define CPL_DISABLE_STDCALL */
