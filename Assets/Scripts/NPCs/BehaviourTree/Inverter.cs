namespace BehaviourTree {
    public class Inverter : Node {
        public Inverter(Node child) {
            this.child = child;
        }

        public override State Evaluate() {
            State childState = child.Evaluate();
            if (childState == State.Success) {
                return State.Failure;
            } else if (childState == State.Failure) {
                return State.Success;
            } else {
                return State.Running;
            }
        }

        public override string ToString() {
            return "Inverter containing: " + child.ToString();
        }
    }
}