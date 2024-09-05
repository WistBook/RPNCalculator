// a, b ∈ R
// 空白文字はスキップされます
//
// 括弧 => ( ... )
//
// 二項演算子
//  加　　算 => a + b
//  減　　算 => a - b
//  乗　　算 => a * b
//  除　　算 => a / b
//  冪乗演算 => a ^ b
//  剰余演算
//
// 単項演算子
//  正の符号 => (+a)
//  負の符号 => (-a) 


class Program{
    static int Precedence(char op){
        switch(op){
            case '(':case ')':
                return 0;
            case '+':case '-':
                return 1;
            case '*':case '/':case '%':
                return 2;
            case '^':
                return 3;
            default :
                throw new Exception("Precedence()内で発生した予期しない例外");
        }
    }
    static Queue<string> ConvertToRPN(string str){
        Queue<string> queue = new Queue<string>();
        Stack<char> stack = new Stack<char>();

        bool detecting = false;
        string temp;
        int i = 0;
        while(i < str.Length){
            if(Char.IsWhiteSpace(str[i])){
                i++;
                continue;
            }
            if(detecting){
                switch(str[i]){
                    case '+':
                        detecting = false;
                        i++;
                        continue;
                    case '-':
                        temp = "-";
                        i++;
                        while(i < str.Length && ( Char.IsDigit(str[i]) || str[i] == '.')){
                            temp += str[i];
                            i++;
                        }
                        queue.Enqueue(temp);
                        detecting = false;
                        continue;
                    default :
                        break;
                }
            }
            switch(str[i]){
                case char c when Char.IsDigit(c):
                    temp = "";
                    Number:
                    while(i < str.Length && ( Char.IsDigit(str[i]) || str[i] == '.')){
                        temp += str[i];
                        i++;
                    }
                    queue.Enqueue(temp);
                    detecting = false;
                    break;
                case '(':
                    stack.Push(str[i]);
                    i++;
                    detecting = true;
                    break;
                case ')'://\\\\\\\\\\\\\\\\\\\\\\\\\ ()は無効な入力
                    if(detecting) throw new ArgumentException("()内にデータがありません");
                    while(stack.Count > 0 && stack.Peek() != '('){
                        queue.Enqueue(stack.Pop().ToString());
                    }
                    stack.Pop();
                    i++;
                    break;
                default:
                    if(str[i] == '+' || str[i] == '-' || str[i] == '*' || str[i] == '/' || str[i] == '^' || str[i] == '%'){
                        if(queue.Count == 0){
                            if(stack.Count == 0){
                                if(str[i] == '-'){
                                    temp = "-";
                                    i++;
                                    goto Number;
                                }
                                else if(str[i] == '+'){
                                    i++;
                                    continue;
                                }
                            }
                        }//\\\\\\\\\\\\\\\\\\\\\\ (* ,(/ ,(^ は無効な入力
                        if(detecting) throw new ArgumentException($"{str[i]}は単項演算子として使用できません");
                        while(stack.Count > 0 && Precedence(stack.Peek()) >= Precedence(str[i])){
                            queue.Enqueue(stack.Pop().ToString());
                        }
                        stack.Push(str[i]);
                        i++;
                        break;
                    }
                    else{
                        throw new ArgumentException($"{str[i]}は演算子として使用できません");
                    }
            }
        }
        while(stack.Count > 0){
            queue.Enqueue(stack.Pop().ToString());
        }
        return queue;
    }

    static void Calculate(in Queue<string> queue,out double result){
        Stack<double> stack = new Stack<double>();
        var i = 0;
        double dtemp , pop1 , pop2;
        string stemp;
        while(i < queue.Count){
            stemp = queue.Dequeue();
            if(double.TryParse(stemp,out dtemp)){
                stack.Push(dtemp);
            }
            else{
                pop1 = stack.Pop();
                pop2 = stack.Pop();
                switch(stemp){
                    case "+":
                        stack.Push(pop2 + pop1);
                        break;
                    case "-":
                        stack.Push(pop2 - pop1);
                        break;
                    case "*":
                        stack.Push(pop2 * pop1);
                        break;
                    case "/":
                        stack.Push(pop2 / pop1);
                        break;
                    case "%":
                        stack.Push(pop2 % pop1);
                        break;
                    case "^":
                        stack.Push(Math.Pow(pop2,pop1));
                        break;
                }
            }
        }
        result = stack.Pop();
        return;
    }

    static bool TryCalculate(in Queue<string> queue,out int result){
        Stack<int> stack = new Stack<int>();
        result = 0;
        var i = 0;
        int itemp , pop1, pop2;
        string stemp;
        while(i < queue.Count){
            stemp = queue.Dequeue();
            if(stemp.Contains('.')) return false;
            if(int.TryParse(stemp,out itemp)){
                stack.Push(itemp);
            }
            else{
                pop1 = stack.Pop();
                pop2 = stack.Pop();
                switch(stemp){
                    case "+":
                        stack.Push(pop2 + pop1);
                        break;
                    case "-":
                        stack.Push(pop2 - pop1);
                        break;
                    case "*":
                        stack.Push(pop2 * pop1);
                        break;
                    case "/":
                        if(pop2 % pop1 != 0) return false;
                        stack.Push(pop2 / pop1);
                        break;
                    case "^":
                        if(pop1 < 1) return false;
                        stack.Push(Pow(pop2,pop1));
                        break;
                }
            }
        }
        result = stack.Pop();
        return true;
    }
    static int Pow(int a, int b){
        int result = 1;
        for(var i = 0; i < b; i++){
            result *= a;
        }
        return result;
    }

    static void Main(){

        Console.Write("計算式を入力してください ");
        string input = Console.ReadLine()!.Trim();
        bool isDouble = input.Contains('.');

        try{
            var converted = ConvertToRPN(input);
            Console.WriteLine(String.Join(" ", converted));

            if(isDouble){
                Calculate(converted,out double result);
                Console.WriteLine(result);
            }else{
                var copied = new Queue<string>(converted);
                var isSuccessful = TryCalculate(converted,out int result);
                if(isSuccessful) Console.WriteLine(result);
                else{
                    Calculate(copied,out double newResult);
                    Console.WriteLine(newResult);
                }
            }
            return;
        }
        catch(ArgumentException e){
            Console.WriteLine("{0}: {1}",e.GetType().Name,e.Message);
            return;
        }
    }
}