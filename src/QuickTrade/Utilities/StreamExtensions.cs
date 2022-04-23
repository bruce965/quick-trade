// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/util/raw/master/LICENSE

using System.Runtime.CompilerServices;

namespace QuickTrade.Utilities;

static class StreamExtensions
{
	class StreamNoDispose : Stream, IAsyncDisposable, IDisposable
	{
		Stream _wrapped;

		public override bool CanRead => _wrapped.CanRead;

		public override bool CanSeek => _wrapped.CanSeek;

		public override bool CanTimeout => _wrapped.CanTimeout;

		public override bool CanWrite => _wrapped.CanWrite;

		public override long Length => _wrapped.Length;

		public override long Position
		{
			get => _wrapped.Position;
			set => _wrapped.Position = value;
		}

		public override int ReadTimeout
		{
			get => _wrapped.ReadTimeout;
			set => _wrapped.ReadTimeout = value;
		}

		public override int WriteTimeout
		{
			get => _wrapped.WriteTimeout;
			set => _wrapped.WriteTimeout = value;
		}

		public StreamNoDispose(Stream stream)
		{
			_wrapped = stream;
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
			=> _wrapped.BeginRead(buffer, offset, count, callback, state);

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
			=> _wrapped.BeginWrite(buffer, offset, count, callback, state);

		public override void Close()
		{
			_wrapped.Close();
		}

		public override void CopyTo(Stream destination, int bufferSize)
			=> CopyTo(destination, bufferSize);

		public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
			=> _wrapped.CopyToAsync(destination, bufferSize, cancellationToken);
		
		void IDisposable.Dispose() { }

		protected override void Dispose(bool disposing) { }

		public override ValueTask DisposeAsync()
			=> ValueTask.CompletedTask;

		public override int EndRead(IAsyncResult asyncResult)
			=> _wrapped.EndRead(asyncResult);

		public override void EndWrite(IAsyncResult asyncResult)
			=> _wrapped.EndWrite(asyncResult);

		public override void Flush()
			=> _wrapped.Flush();

		public override Task FlushAsync(CancellationToken cancellationToken)
			=> _wrapped.FlushAsync(cancellationToken);

		public override int Read(byte[] buffer, int offset, int count)
			=> _wrapped.Read(buffer, offset, count);

		public override int Read(Span<byte> buffer)
			=> _wrapped.Read(buffer);

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
			=> _wrapped.ReadAsync(buffer, offset, count, cancellationToken);

		public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
			=> _wrapped.ReadAsync(buffer, cancellationToken);

		public override int ReadByte()
			=> _wrapped.ReadByte();

		public override long Seek(long offset, SeekOrigin origin)
			=> _wrapped.Seek(offset, origin);

		public override void SetLength(long value)
			=> _wrapped.SetLength(value);

		public override void Write(byte[] buffer, int offset, int count)
			=> _wrapped.Write(buffer, offset, count);

		public override void Write(ReadOnlySpan<byte> buffer)
			=> _wrapped.Write(buffer);

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
			=> _wrapped.WriteAsync(buffer, offset, count, cancellationToken);

		public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
			=> _wrapped.WriteAsync(buffer, cancellationToken);

		public override void WriteByte(byte value)
			=> _wrapped.WriteByte(value);
	}

	static readonly ConditionalWeakTable<Stream, StreamNoDispose> _noDispose = new();

	/// <summary>
	/// Wraps a stream into a stream that ignores dispose requests.
	/// </summary>
	/// <param name="stream">Wrapped stream.</param>
	/// <returns>Stream wrapper.</returns>
	public static Stream NoDispose(this Stream stream)
		=> _noDispose.GetValue(stream, stream => new StreamNoDispose(stream));
}
