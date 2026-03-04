using NUnit.Framework;
using SOSXR.SeaShark;


namespace SOSXR.SeaShark.Tests
{
    /// <summary>
    /// Test double that records Execute/Undo/Redo call counts for assertion.
    /// </summary>
    internal class CountingCommand : ICommand
    {
        public int ExecuteCount;
        public int UndoCount;
        public int RedoCount;

        public void Execute() => ExecuteCount++;
        public void Undo()    => UndoCount++;
        public void Redo()    => RedoCount++;
    }


    [TestFixture]
    public class CommandQueueTests
    {
        private CommandQueue _queue;
        private CountingCommand _cmd;

        [SetUp]
        public void SetUp()
        {
            _queue = new CommandQueue();
            _cmd   = new CountingCommand();
        }


        [Test]
        public void Execute_CallsCommandExecute()
        {
            _queue.Execute(_cmd);
            Assert.AreEqual(1, _cmd.ExecuteCount);
        }

        [Test]
        public void Undo_OnEmpty_DoesNotThrowOrCallUndo()
        {
            Assert.DoesNotThrow(() => _queue.Undo());
            Assert.AreEqual(0, _cmd.UndoCount);
        }

        [Test]
        public void Undo_AfterExecute_CallsCommandUndo()
        {
            _queue.Execute(_cmd);
            _queue.Undo();
            Assert.AreEqual(1, _cmd.UndoCount);
        }

        [Test]
        public void Redo_OnEmptyRedoStack_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _queue.Redo());
        }

        [Test]
        public void Redo_AfterUndo_ReExecutesCommand()
        {
            _queue.Execute(_cmd);
            _queue.Undo();
            _queue.Redo();
            Assert.AreEqual(2, _cmd.ExecuteCount);
        }

        [Test]
        public void Execute_AfterUndo_ClearsRedoStack()
        {
            var cmd2 = new CountingCommand();
            _queue.Execute(_cmd);
            _queue.Undo();
            // This new Execute should clear the redo stack
            _queue.Execute(cmd2);
            _queue.Redo(); // should be a no-op
            Assert.AreEqual(1, _cmd.ExecuteCount, "Original command should not have been re-executed");
        }

        [Test]
        public void Clear_PreventsUndo()
        {
            _queue.Execute(_cmd);
            _queue.Clear();
            _queue.Undo();
            Assert.AreEqual(0, _cmd.UndoCount);
        }

        [Test]
        public void Queue_IsFIFO_UndoesFirstExecutedFirst()
        {
            var cmd1 = new CountingCommand();
            var cmd2 = new CountingCommand();

            _queue.Execute(cmd1);
            _queue.Execute(cmd2);

            _queue.Undo(); // should undo cmd1 (FIFO dequeue)

            Assert.AreEqual(1, cmd1.UndoCount, "FIFO: first enqueued should be first dequeued");
            Assert.AreEqual(0, cmd2.UndoCount);
        }

        [Test]
        public void MultipleUndoRedo_MaintainsCorrectOrder()
        {
            var cmd1 = new CountingCommand();
            var cmd2 = new CountingCommand();

            _queue.Execute(cmd1);
            _queue.Execute(cmd2);
            _queue.Undo(); // undoes cmd1
            _queue.Undo(); // undoes cmd2
            _queue.Redo(); // re-executes cmd1
            _queue.Redo(); // re-executes cmd2

            Assert.AreEqual(2, cmd1.ExecuteCount);
            Assert.AreEqual(2, cmd2.ExecuteCount);
        }
    }
}
