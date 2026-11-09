using NUnit.Framework;
using SOSXR.SeaShark;


namespace SOSXR.SeaShark.Tests
{
    [TestFixture]
    public class CommandStackTests
    {
        private CommandStack _stack;
        private CountingCommand _cmd;

        [SetUp]
        public void SetUp()
        {
            _stack = new CommandStack();
            _cmd   = new CountingCommand();
        }


        [Test]
        public void Execute_CallsCommandExecute()
        {
            _stack.Execute(_cmd);
            Assert.AreEqual(1, _cmd.ExecuteCount);
        }

        [Test]
        public void Undo_OnEmpty_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _stack.Undo());
        }

        [Test]
        public void Undo_AfterExecute_CallsCommandUndo()
        {
            _stack.Execute(_cmd);
            _stack.Undo();
            Assert.AreEqual(1, _cmd.UndoCount);
        }

        [Test]
        public void Redo_OnEmptyRedoQueue_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _stack.Redo());
        }

        [Test]
        public void Redo_AfterUndo_ReExecutesCommand()
        {
            _stack.Execute(_cmd);
            _stack.Undo();
            _stack.Redo();
            Assert.AreEqual(2, _cmd.ExecuteCount);
        }

        [Test]
        public void Execute_AfterUndo_ClearsRedoQueue()
        {
            var cmd2 = new CountingCommand();
            _stack.Execute(_cmd);
            _stack.Undo();
            _stack.Execute(cmd2);
            _stack.Redo(); // should be no-op
            Assert.AreEqual(1, _cmd.ExecuteCount, "Redo stack should be cleared after new Execute");
        }

        [Test]
        public void Clear_PreventsUndo()
        {
            _stack.Execute(_cmd);
            _stack.Clear();
            _stack.Undo();
            Assert.AreEqual(0, _cmd.UndoCount);
        }

        [Test]
        public void Stack_IsLIFO_UndoesMostRecentFirst()
        {
            var cmd1 = new CountingCommand();
            var cmd2 = new CountingCommand();

            _stack.Execute(cmd1);
            _stack.Execute(cmd2);

            _stack.Undo(); // should undo cmd2 (LIFO)

            Assert.AreEqual(0, cmd1.UndoCount, "LIFO: most recent should be undone first");
            Assert.AreEqual(1, cmd2.UndoCount);
        }

        [Test]
        public void MultipleUndoRedo_MaintainsLIFOOrder()
        {
            var cmd1 = new CountingCommand();
            var cmd2 = new CountingCommand();

            _stack.Execute(cmd1);
            _stack.Execute(cmd2);

            _stack.Undo(); // undoes cmd2
            _stack.Undo(); // undoes cmd1

            _stack.Redo(); // re-executes cmd1 (most recently undone)
            _stack.Redo(); // re-executes cmd2

            Assert.AreEqual(2, cmd1.ExecuteCount);
            Assert.AreEqual(2, cmd2.ExecuteCount);
        }

        [Test]
        public void Redo_ReExecutesMostRecentlyUndoneFirst()
        {
            var log = new System.Collections.Generic.List<string>();
            var cmd1 = new LoggingCommand("cmd1", log);
            var cmd2 = new LoggingCommand("cmd2", log);

            _stack.Execute(cmd1);
            _stack.Execute(cmd2);
            _stack.Undo(); // undoes cmd2
            _stack.Undo(); // undoes cmd1
            log.Clear();

            _stack.Redo();
            _stack.Redo();

            // LIFO redo: cmd1 was undone last, so it must be redone first (FIFO would give cmd2, cmd1).
            CollectionAssert.AreEqual(new[] { "cmd1", "cmd2" }, log);
        }


        private sealed class LoggingCommand : ICommand
        {
            private readonly string _name;
            private readonly System.Collections.Generic.List<string> _log;

            public LoggingCommand(string name, System.Collections.Generic.List<string> log)
            {
                _name = name;
                _log = log;
            }

            public void Execute() => _log.Add(_name);
            public void Undo() { }
            public void Redo() => _log.Add(_name);
        }
    }
}
