namespace BehaviourTree {
    public class Sequence : Node {
        public Node[] nodes;
        public int current = 0;
        public Sequence(Node[] nodes) {
            this.nodes = nodes;
        }
        
        public override State Evaluate() {
            State childState = nodes[current].Evaluate();
            // if the current node succeeded, report that success upwards
            // and move to the next node.
            if (childState == State.Success) {
                current++;

                // reset to beginning if we have finished the sequence
                if (current >= nodes.Length) {
                    current = 0;
                    return State.Success;
                } else {
                    return Evaluate();
                }

            // if the current node is still running, keep running
            } else if (childState == State.Running) {
                current = 0;
                return State.Running;

            // if the current node failed, then return a failure and return to the
            // beginning of the sequence
            } else {
                current = 0;
                return State.Failure;
            }
        }
    }

}