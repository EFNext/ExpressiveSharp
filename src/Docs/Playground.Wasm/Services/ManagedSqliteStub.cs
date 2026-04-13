// ManagedSqliteStub — a fully-managed SQLitePCL.ISQLite3Provider implementation
// that EF Core can interrogate at startup *without* needing the native sqlite3
// engine. The playground only ever calls ToQueryString() on a queryable; that
// path goes through EF Core's relational query translator and never executes
// real SQL, but the SqliteUpdateSqlGenerator constructor probes
// connection.ServerVersion → SQLitePCL.raw.sqlite3_libversion() during DI
// graph construction. This stub answers the metadata calls that EF Core makes
// during model build, and throws NotSupportedException for anything that would
// require running real queries.
//
// The interface has 152 methods (utf8z is a ref struct so DispatchProxy can't
// be used). Most just throw — only a handful of "metadata" calls have real
// answers. Generated from reflection at design time and committed.

#pragma warning disable IDE0060 // Unused parameters — interface forces us to declare them

using SQLitePCL;

namespace ExpressiveSharp.Docs.Playground.Wasm.Services;

public sealed class ManagedSqliteStub : ISQLite3Provider
{
    /// <summary>
    /// Registers this stub as the global SQLitePCL provider. Idempotent.
    /// Must be called before any DbContext that uses Sqlite.Core is constructed.
    /// </summary>
    public static void Register()
    {
        try
        {
            SQLitePCL.raw.SetProvider(new ManagedSqliteStub());
        }
        catch (System.InvalidOperationException)
        {
            // SetProvider throws if a provider is already registered. Tolerate
            // double-registration so the stub can be installed eagerly from
            // multiple host entrypoints (Program.cs, test setup, etc).
        }
    }

    // ── Metadata calls EF Core makes during DbContext init ─────────────────

    // Reported as the SQLite library version. EF Core uses this for feature
    // detection (RETURNING clause support added in 3.35). Reporting a recent
    // version unlocks all relational translator features.
    public utf8z sqlite3_libversion() => utf8z.FromString("3.45.0");

    // Companion to libversion. Format is major*1_000_000 + minor*1_000 + patch.
    public int sqlite3_libversion_number() => 3045000;

    public utf8z sqlite3_sourceid() => utf8z.FromString("managed-stub-no-sourceid");

    // 1 = single-threaded. WASM is single-threaded anyway.
    public int sqlite3_threadsafe() => 1;

    public string GetNativeLibraryName() => "managed-stub";

    // SQLitePCL.raw initializes by calling these once. They must succeed.
    public int sqlite3_initialize() => 0; // SQLITE_OK
    public int sqlite3_shutdown() => 0;
    public int sqlite3_config_log(global::SQLitePCL.delegate_log @func, global::System.Object @v) => 0;
    public int sqlite3_enable_shared_cache(int @enable) => 0;

    // ── Everything else throws ─────────────────────────────────────────────

    private static System.Exception NotSupported(string method) =>
        new System.NotSupportedException(
            $"ManagedSqliteStub does not implement '{method}'. " +
            "The Playground only supports ToQueryString() — query execution is not allowed.");

    public int sqlite3__vfs__delete(global::SQLitePCL.utf8z @vfs, global::SQLitePCL.utf8z @pathname, int @syncDir) => throw NotSupported("sqlite3__vfs__delete");
    public int sqlite3_backup_finish(global::System.IntPtr @backup) => throw NotSupported("sqlite3_backup_finish");
    public global::SQLitePCL.sqlite3_backup sqlite3_backup_init(global::SQLitePCL.sqlite3 @destDb, global::SQLitePCL.utf8z @destName, global::SQLitePCL.sqlite3 @sourceDb, global::SQLitePCL.utf8z @sourceName) => throw NotSupported("sqlite3_backup_init");
    public int sqlite3_backup_pagecount(global::SQLitePCL.sqlite3_backup @backup) => throw NotSupported("sqlite3_backup_pagecount");
    public int sqlite3_backup_remaining(global::SQLitePCL.sqlite3_backup @backup) => throw NotSupported("sqlite3_backup_remaining");
    public int sqlite3_backup_step(global::SQLitePCL.sqlite3_backup @backup, int @nPage) => throw NotSupported("sqlite3_backup_step");
    public int sqlite3_bind_blob(global::SQLitePCL.sqlite3_stmt @stmt, int @index, System.ReadOnlySpan<byte> @blob) => throw NotSupported("sqlite3_bind_blob");
    public int sqlite3_bind_double(global::SQLitePCL.sqlite3_stmt @stmt, int @index, double @val) => throw NotSupported("sqlite3_bind_double");
    public int sqlite3_bind_int(global::SQLitePCL.sqlite3_stmt @stmt, int @index, int @val) => throw NotSupported("sqlite3_bind_int");
    public int sqlite3_bind_int64(global::SQLitePCL.sqlite3_stmt @stmt, int @index, long @val) => throw NotSupported("sqlite3_bind_int64");
    public int sqlite3_bind_null(global::SQLitePCL.sqlite3_stmt @stmt, int @index) => throw NotSupported("sqlite3_bind_null");
    public int sqlite3_bind_parameter_count(global::SQLitePCL.sqlite3_stmt @stmt) => throw NotSupported("sqlite3_bind_parameter_count");
    public int sqlite3_bind_parameter_index(global::SQLitePCL.sqlite3_stmt @stmt, global::SQLitePCL.utf8z @strName) => throw NotSupported("sqlite3_bind_parameter_index");
    public global::SQLitePCL.utf8z sqlite3_bind_parameter_name(global::SQLitePCL.sqlite3_stmt @stmt, int @index) => throw NotSupported("sqlite3_bind_parameter_name");
    public int sqlite3_bind_text(global::SQLitePCL.sqlite3_stmt @stmt, int @index, System.ReadOnlySpan<byte> @text) => throw NotSupported("sqlite3_bind_text");
    public int sqlite3_bind_text(global::SQLitePCL.sqlite3_stmt @stmt, int @index, global::SQLitePCL.utf8z @text) => throw NotSupported("sqlite3_bind_text");
    public int sqlite3_bind_text16(global::SQLitePCL.sqlite3_stmt @stmt, int @index, System.ReadOnlySpan<global::System.Char> @text) => throw NotSupported("sqlite3_bind_text16");
    public int sqlite3_bind_zeroblob(global::SQLitePCL.sqlite3_stmt @stmt, int @index, int @size) => throw NotSupported("sqlite3_bind_zeroblob");
    public int sqlite3_blob_bytes(global::SQLitePCL.sqlite3_blob @blob) => throw NotSupported("sqlite3_blob_bytes");
    public int sqlite3_blob_close(global::System.IntPtr @blob) => throw NotSupported("sqlite3_blob_close");
    public int sqlite3_blob_open(global::SQLitePCL.sqlite3 @db, global::SQLitePCL.utf8z @db_utf8, global::SQLitePCL.utf8z @table_utf8, global::SQLitePCL.utf8z @col_utf8, long @rowid, int @flags, out global::SQLitePCL.sqlite3_blob @blob) => throw NotSupported("sqlite3_blob_open");
    public int sqlite3_blob_read(global::SQLitePCL.sqlite3_blob @blob, System.Span<byte> @b, int @offset) => throw NotSupported("sqlite3_blob_read");
    public int sqlite3_blob_reopen(global::SQLitePCL.sqlite3_blob @blob, long @rowid) => throw NotSupported("sqlite3_blob_reopen");
    public int sqlite3_blob_write(global::SQLitePCL.sqlite3_blob @blob, System.ReadOnlySpan<byte> @b, int @offset) => throw NotSupported("sqlite3_blob_write");
    public int sqlite3_busy_timeout(global::SQLitePCL.sqlite3 @db, int @ms) => throw NotSupported("sqlite3_busy_timeout");
    public int sqlite3_changes(global::SQLitePCL.sqlite3 @db) => throw NotSupported("sqlite3_changes");
    public int sqlite3_clear_bindings(global::SQLitePCL.sqlite3_stmt @stmt) => throw NotSupported("sqlite3_clear_bindings");
    public int sqlite3_close(global::System.IntPtr @db) => throw NotSupported("sqlite3_close");
    public int sqlite3_close_v2(global::System.IntPtr @db) => throw NotSupported("sqlite3_close_v2");
    public System.ReadOnlySpan<byte> sqlite3_column_blob(global::SQLitePCL.sqlite3_stmt @stmt, int @index) => throw NotSupported("sqlite3_column_blob");
    public int sqlite3_column_bytes(global::SQLitePCL.sqlite3_stmt @stmt, int @index) => throw NotSupported("sqlite3_column_bytes");
    public int sqlite3_column_count(global::SQLitePCL.sqlite3_stmt @stmt) => throw NotSupported("sqlite3_column_count");
    public global::SQLitePCL.utf8z sqlite3_column_database_name(global::SQLitePCL.sqlite3_stmt @stmt, int @index) => throw NotSupported("sqlite3_column_database_name");
    public global::SQLitePCL.utf8z sqlite3_column_decltype(global::SQLitePCL.sqlite3_stmt @stmt, int @index) => throw NotSupported("sqlite3_column_decltype");
    public double sqlite3_column_double(global::SQLitePCL.sqlite3_stmt @stmt, int @index) => throw NotSupported("sqlite3_column_double");
    public int sqlite3_column_int(global::SQLitePCL.sqlite3_stmt @stmt, int @index) => throw NotSupported("sqlite3_column_int");
    public long sqlite3_column_int64(global::SQLitePCL.sqlite3_stmt @stmt, int @index) => throw NotSupported("sqlite3_column_int64");
    public global::SQLitePCL.utf8z sqlite3_column_name(global::SQLitePCL.sqlite3_stmt @stmt, int @index) => throw NotSupported("sqlite3_column_name");
    public global::SQLitePCL.utf8z sqlite3_column_origin_name(global::SQLitePCL.sqlite3_stmt @stmt, int @index) => throw NotSupported("sqlite3_column_origin_name");
    public global::SQLitePCL.utf8z sqlite3_column_table_name(global::SQLitePCL.sqlite3_stmt @stmt, int @index) => throw NotSupported("sqlite3_column_table_name");
    public global::SQLitePCL.utf8z sqlite3_column_text(global::SQLitePCL.sqlite3_stmt @stmt, int @index) => throw NotSupported("sqlite3_column_text");
    public int sqlite3_column_type(global::SQLitePCL.sqlite3_stmt @stmt, int @index) => throw NotSupported("sqlite3_column_type");
    public void sqlite3_commit_hook(global::SQLitePCL.sqlite3 @db, global::SQLitePCL.delegate_commit @func, global::System.Object @v) => throw NotSupported("sqlite3_commit_hook");
    public global::SQLitePCL.utf8z sqlite3_compileoption_get(int @n) => throw NotSupported("sqlite3_compileoption_get");
    public int sqlite3_compileoption_used(global::SQLitePCL.utf8z @sql) => throw NotSupported("sqlite3_compileoption_used");
    public int sqlite3_complete(global::SQLitePCL.utf8z @sql) => throw NotSupported("sqlite3_complete");
    public int sqlite3_config(int @op) => throw NotSupported("sqlite3_config");
    public int sqlite3_config(int @op, int @val) => throw NotSupported("sqlite3_config(int,int)");
    public int sqlite3_create_collation(global::SQLitePCL.sqlite3 @db, global::System.Byte[] @name, global::System.Object @v, global::SQLitePCL.delegate_collation @func) => throw NotSupported("sqlite3_create_collation");
    public int sqlite3_create_function(global::SQLitePCL.sqlite3 @db, global::System.Byte[] @name, int @nArg, int @flags, global::System.Object @v, global::SQLitePCL.delegate_function_scalar @func) => throw NotSupported("sqlite3_create_function(scalar)");
    public int sqlite3_create_function(global::SQLitePCL.sqlite3 @db, global::System.Byte[] @name, int @nArg, int @flags, global::System.Object @v, global::SQLitePCL.delegate_function_aggregate_step @func_step, global::SQLitePCL.delegate_function_aggregate_final @func_final) => throw NotSupported("sqlite3_create_function(aggregate)");
    public int sqlite3_data_count(global::SQLitePCL.sqlite3_stmt @stmt) => throw NotSupported("sqlite3_data_count");
    public int sqlite3_db_config(global::SQLitePCL.sqlite3 @db, int @op, global::SQLitePCL.utf8z @val) => throw NotSupported("sqlite3_db_config(utf8z)");
    public int sqlite3_db_config(global::SQLitePCL.sqlite3 @db, int @op, int @val, out int @result) => throw NotSupported("sqlite3_db_config(int,out int)");
    public int sqlite3_db_config(global::SQLitePCL.sqlite3 @db, int @op, global::System.IntPtr @ptr, int @int0, int @int1) => throw NotSupported("sqlite3_db_config(IntPtr)");
    public global::SQLitePCL.utf8z sqlite3_db_filename(global::SQLitePCL.sqlite3 @db, global::SQLitePCL.utf8z @att) => throw NotSupported("sqlite3_db_filename");
    public global::System.IntPtr sqlite3_db_handle(global::System.IntPtr @stmt) => throw NotSupported("sqlite3_db_handle");
    public int sqlite3_db_readonly(global::SQLitePCL.sqlite3 @db, global::SQLitePCL.utf8z @dbName) => throw NotSupported("sqlite3_db_readonly");
    public int sqlite3_db_status(global::SQLitePCL.sqlite3 @db, int @op, out int @current, out int @highest, int @resetFlg) => throw NotSupported("sqlite3_db_status");
    public int sqlite3_deserialize(global::SQLitePCL.sqlite3 @db, global::SQLitePCL.utf8z @schema, global::System.IntPtr @data, long @szDb, long @szBuf, int @flags) => throw NotSupported("sqlite3_deserialize");
    public int sqlite3_enable_load_extension(global::SQLitePCL.sqlite3 @db, int @enable) => throw NotSupported("sqlite3_enable_load_extension");
    public int sqlite3_errcode(global::SQLitePCL.sqlite3 @db) => throw NotSupported("sqlite3_errcode");
    public global::SQLitePCL.utf8z sqlite3_errmsg(global::SQLitePCL.sqlite3 @db) => throw NotSupported("sqlite3_errmsg");
    public global::SQLitePCL.utf8z sqlite3_errstr(int @rc) => throw NotSupported("sqlite3_errstr");
    public int sqlite3_exec(global::SQLitePCL.sqlite3 @db, global::SQLitePCL.utf8z @sql, global::SQLitePCL.delegate_exec @callback, global::System.Object @user_data, out global::System.IntPtr @errMsg) => throw NotSupported("sqlite3_exec");
    public int sqlite3_extended_errcode(global::SQLitePCL.sqlite3 @db) => throw NotSupported("sqlite3_extended_errcode");
    public int sqlite3_extended_result_codes(global::SQLitePCL.sqlite3 @db, int @onoff) => throw NotSupported("sqlite3_extended_result_codes");
    public int sqlite3_finalize(global::System.IntPtr @stmt) => throw NotSupported("sqlite3_finalize");
    public void sqlite3_free(global::System.IntPtr @p) => throw NotSupported("sqlite3_free");
    public int sqlite3_get_autocommit(global::SQLitePCL.sqlite3 @db) => throw NotSupported("sqlite3_get_autocommit");
    public long sqlite3_hard_heap_limit64(long @n) => throw NotSupported("sqlite3_hard_heap_limit64");
    public void sqlite3_interrupt(global::SQLitePCL.sqlite3 @db) => throw NotSupported("sqlite3_interrupt");
    public int sqlite3_key(global::SQLitePCL.sqlite3 @db, System.ReadOnlySpan<byte> @key) => throw NotSupported("sqlite3_key");
    public int sqlite3_key_v2(global::SQLitePCL.sqlite3 @db, global::SQLitePCL.utf8z @dbname, System.ReadOnlySpan<byte> @key) => throw NotSupported("sqlite3_key_v2");
    public int sqlite3_keyword_count() => throw NotSupported("sqlite3_keyword_count");
    public int sqlite3_keyword_name(int @i, out string @name) => throw NotSupported("sqlite3_keyword_name");
    public long sqlite3_last_insert_rowid(global::SQLitePCL.sqlite3 @db) => throw NotSupported("sqlite3_last_insert_rowid");
    public int sqlite3_limit(global::SQLitePCL.sqlite3 @db, int @id, int @newVal) => throw NotSupported("sqlite3_limit");
    public int sqlite3_load_extension(global::SQLitePCL.sqlite3 @db, global::SQLitePCL.utf8z @zFile, global::SQLitePCL.utf8z @zProc, out global::SQLitePCL.utf8z @pzErrMsg) => throw NotSupported("sqlite3_load_extension");
    public void sqlite3_log(int @errcode, global::SQLitePCL.utf8z @s) => throw NotSupported("sqlite3_log");
    public global::System.IntPtr sqlite3_malloc(int @n) => throw NotSupported("sqlite3_malloc");
    public global::System.IntPtr sqlite3_malloc64(long @n) => throw NotSupported("sqlite3_malloc64");
    public long sqlite3_memory_highwater(int @resetFlag) => throw NotSupported("sqlite3_memory_highwater");
    public long sqlite3_memory_used() => throw NotSupported("sqlite3_memory_used");
    public global::System.IntPtr sqlite3_next_stmt(global::SQLitePCL.sqlite3 @db, global::System.IntPtr @stmt) => throw NotSupported("sqlite3_next_stmt");
    public int sqlite3_open(global::SQLitePCL.utf8z @filename, out global::System.IntPtr @db) => throw NotSupported("sqlite3_open");
    public int sqlite3_open_v2(global::SQLitePCL.utf8z @filename, out global::System.IntPtr @db, int @flags, global::SQLitePCL.utf8z @vfs) => throw NotSupported("sqlite3_open_v2");
    public int sqlite3_prepare_v2(global::SQLitePCL.sqlite3 @db, System.ReadOnlySpan<byte> @sql, out global::System.IntPtr @stmt, out System.ReadOnlySpan<byte> @remain) => throw NotSupported("sqlite3_prepare_v2(span)");
    public int sqlite3_prepare_v2(global::SQLitePCL.sqlite3 @db, global::SQLitePCL.utf8z @sql, out global::System.IntPtr @stmt, out global::SQLitePCL.utf8z @remain) => throw NotSupported("sqlite3_prepare_v2(utf8z)");
    public int sqlite3_prepare_v3(global::SQLitePCL.sqlite3 @db, System.ReadOnlySpan<byte> @sql, uint @flags, out global::System.IntPtr @stmt, out System.ReadOnlySpan<byte> @remain) => throw NotSupported("sqlite3_prepare_v3(span)");
    public int sqlite3_prepare_v3(global::SQLitePCL.sqlite3 @db, global::SQLitePCL.utf8z @sql, uint @flags, out global::System.IntPtr @stmt, out global::SQLitePCL.utf8z @remain) => throw NotSupported("sqlite3_prepare_v3(utf8z)");
    public void sqlite3_profile(global::SQLitePCL.sqlite3 @db, global::SQLitePCL.delegate_profile @func, global::System.Object @v) => throw NotSupported("sqlite3_profile");
    public void sqlite3_progress_handler(global::SQLitePCL.sqlite3 @db, int @instructions, global::SQLitePCL.delegate_progress @func, global::System.Object @v) => throw NotSupported("sqlite3_progress_handler");
    public int sqlite3_rekey(global::SQLitePCL.sqlite3 @db, System.ReadOnlySpan<byte> @key) => throw NotSupported("sqlite3_rekey");
    public int sqlite3_rekey_v2(global::SQLitePCL.sqlite3 @db, global::SQLitePCL.utf8z @dbname, System.ReadOnlySpan<byte> @key) => throw NotSupported("sqlite3_rekey_v2");
    public int sqlite3_reset(global::SQLitePCL.sqlite3_stmt @stmt) => throw NotSupported("sqlite3_reset");
    public void sqlite3_result_blob(global::System.IntPtr @context, System.ReadOnlySpan<byte> @val) => throw NotSupported("sqlite3_result_blob");
    public void sqlite3_result_double(global::System.IntPtr @context, double @val) => throw NotSupported("sqlite3_result_double");
    public void sqlite3_result_error(global::System.IntPtr @context, System.ReadOnlySpan<byte> @strErr) => throw NotSupported("sqlite3_result_error(span)");
    public void sqlite3_result_error(global::System.IntPtr @context, global::SQLitePCL.utf8z @strErr) => throw NotSupported("sqlite3_result_error(utf8z)");
    public void sqlite3_result_error_code(global::System.IntPtr @context, int @code) => throw NotSupported("sqlite3_result_error_code");
    public void sqlite3_result_error_nomem(global::System.IntPtr @context) => throw NotSupported("sqlite3_result_error_nomem");
    public void sqlite3_result_error_toobig(global::System.IntPtr @context) => throw NotSupported("sqlite3_result_error_toobig");
    public void sqlite3_result_int(global::System.IntPtr @context, int @val) => throw NotSupported("sqlite3_result_int");
    public void sqlite3_result_int64(global::System.IntPtr @context, long @val) => throw NotSupported("sqlite3_result_int64");
    public void sqlite3_result_null(global::System.IntPtr @context) => throw NotSupported("sqlite3_result_null");
    public void sqlite3_result_text(global::System.IntPtr @context, System.ReadOnlySpan<byte> @val) => throw NotSupported("sqlite3_result_text(span)");
    public void sqlite3_result_text(global::System.IntPtr @context, global::SQLitePCL.utf8z @val) => throw NotSupported("sqlite3_result_text(utf8z)");
    public void sqlite3_result_zeroblob(global::System.IntPtr @context, int @n) => throw NotSupported("sqlite3_result_zeroblob");
    public void sqlite3_rollback_hook(global::SQLitePCL.sqlite3 @db, global::SQLitePCL.delegate_rollback @func, global::System.Object @v) => throw NotSupported("sqlite3_rollback_hook");
    public global::System.IntPtr sqlite3_serialize(global::SQLitePCL.sqlite3 @db, global::SQLitePCL.utf8z @schema, out long @size, int @flags) => throw NotSupported("sqlite3_serialize");
    public int sqlite3_set_authorizer(global::SQLitePCL.sqlite3 @db, global::SQLitePCL.delegate_authorizer @authorizer, global::System.Object @user_data) => throw NotSupported("sqlite3_set_authorizer");
    public int sqlite3_snapshot_cmp(global::SQLitePCL.sqlite3_snapshot @p1, global::SQLitePCL.sqlite3_snapshot @p2) => throw NotSupported("sqlite3_snapshot_cmp");
    public void sqlite3_snapshot_free(global::System.IntPtr @snap) => throw NotSupported("sqlite3_snapshot_free");
    public int sqlite3_snapshot_get(global::SQLitePCL.sqlite3 @db, global::SQLitePCL.utf8z @schema, out global::System.IntPtr @snap) => throw NotSupported("sqlite3_snapshot_get");
    public int sqlite3_snapshot_open(global::SQLitePCL.sqlite3 @db, global::SQLitePCL.utf8z @schema, global::SQLitePCL.sqlite3_snapshot @snap) => throw NotSupported("sqlite3_snapshot_open");
    public int sqlite3_snapshot_recover(global::SQLitePCL.sqlite3 @db, global::SQLitePCL.utf8z @name) => throw NotSupported("sqlite3_snapshot_recover");
    public long sqlite3_soft_heap_limit64(long @n) => throw NotSupported("sqlite3_soft_heap_limit64");
    public global::SQLitePCL.utf8z sqlite3_sql(global::SQLitePCL.sqlite3_stmt @stmt) => throw NotSupported("sqlite3_sql");
    public int sqlite3_status(int @op, out int @current, out int @highwater, int @resetFlag) => throw NotSupported("sqlite3_status");
    public int sqlite3_step(global::SQLitePCL.sqlite3_stmt @stmt) => throw NotSupported("sqlite3_step");
    public int sqlite3_stmt_busy(global::SQLitePCL.sqlite3_stmt @stmt) => throw NotSupported("sqlite3_stmt_busy");
    public int sqlite3_stmt_isexplain(global::SQLitePCL.sqlite3_stmt @stmt) => throw NotSupported("sqlite3_stmt_isexplain");
    public int sqlite3_stmt_readonly(global::SQLitePCL.sqlite3_stmt @stmt) => throw NotSupported("sqlite3_stmt_readonly");
    public int sqlite3_stmt_status(global::SQLitePCL.sqlite3_stmt @stmt, int @op, int @resetFlg) => throw NotSupported("sqlite3_stmt_status");
    public int sqlite3_stricmp(global::System.IntPtr @p, global::System.IntPtr @q) => throw NotSupported("sqlite3_stricmp");
    public int sqlite3_strnicmp(global::System.IntPtr @p, global::System.IntPtr @q, int @n) => throw NotSupported("sqlite3_strnicmp");
    public int sqlite3_table_column_metadata(global::SQLitePCL.sqlite3 @db, global::SQLitePCL.utf8z @dbName, global::SQLitePCL.utf8z @tblName, global::SQLitePCL.utf8z @colName, out global::SQLitePCL.utf8z @dataType, out global::SQLitePCL.utf8z @collSeq, out int @notNull, out int @primaryKey, out int @autoInc) => throw NotSupported("sqlite3_table_column_metadata");
    public int sqlite3_total_changes(global::SQLitePCL.sqlite3 @db) => throw NotSupported("sqlite3_total_changes");
    public void sqlite3_trace(global::SQLitePCL.sqlite3 @db, global::SQLitePCL.delegate_trace @func, global::System.Object @v) => throw NotSupported("sqlite3_trace");
    public void sqlite3_update_hook(global::SQLitePCL.sqlite3 @db, global::SQLitePCL.delegate_update @func, global::System.Object @v) => throw NotSupported("sqlite3_update_hook");
    public System.ReadOnlySpan<byte> sqlite3_value_blob(global::System.IntPtr @p) => throw NotSupported("sqlite3_value_blob");
    public int sqlite3_value_bytes(global::System.IntPtr @p) => throw NotSupported("sqlite3_value_bytes");
    public double sqlite3_value_double(global::System.IntPtr @p) => throw NotSupported("sqlite3_value_double");
    public int sqlite3_value_int(global::System.IntPtr @p) => throw NotSupported("sqlite3_value_int");
    public long sqlite3_value_int64(global::System.IntPtr @p) => throw NotSupported("sqlite3_value_int64");
    public global::SQLitePCL.utf8z sqlite3_value_text(global::System.IntPtr @p) => throw NotSupported("sqlite3_value_text");
    public int sqlite3_value_type(global::System.IntPtr @p) => throw NotSupported("sqlite3_value_type");
    public int sqlite3_wal_autocheckpoint(global::SQLitePCL.sqlite3 @db, int @n) => throw NotSupported("sqlite3_wal_autocheckpoint");
    public int sqlite3_wal_checkpoint(global::SQLitePCL.sqlite3 @db, global::SQLitePCL.utf8z @dbName) => throw NotSupported("sqlite3_wal_checkpoint");
    public int sqlite3_wal_checkpoint_v2(global::SQLitePCL.sqlite3 @db, global::SQLitePCL.utf8z @dbName, int @eMode, out int @logSize, out int @framesCheckPointed) => throw NotSupported("sqlite3_wal_checkpoint_v2");
    public int sqlite3_win32_set_directory(int @typ, global::SQLitePCL.utf8z @path) => throw NotSupported("sqlite3_win32_set_directory");
}
