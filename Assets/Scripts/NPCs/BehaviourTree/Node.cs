namespace BehaviourTree {
    public enum State {
        Success,
        Failure,
        Running
    }

    public class Node {
        protected Node child;
        private System.Func<State> action;

        public Node() {}

        public Node(System.Func<State> action) {
            this.action = action;
        }
        
        public Node(System.Func<State> action, Node child) {
            this.action = action;
            this.child = child; 
        }

        public virtual State Evaluate() {
            State state = action.Invoke();
            if (child != null && state == State.Success) {
                return child.Evaluate();
            }
            return state;
        }

        public override string ToString() {
            if (action != null) 
                return "Leaf Node containing: " + action.Method.Name;
            else
                return "Leaf Node";
        }
    }
}