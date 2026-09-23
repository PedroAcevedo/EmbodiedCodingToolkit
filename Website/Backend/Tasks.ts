interface Task {
  id: string;
  title: string;
  description: string;
  example: string;
  functionName: string;
  variables: string[];
  testCases: TestCases[];
}

export interface TestCases {
  inputs: string[];
  output: string;
}

export const allTasks: Task[] = [
  {
      "id": "caesar_cipher",
      "title": "Task 1: Caesar Cipher",
      "description": "Encrypt a non-empty list of uppercase letters using a Caesar cipher.\n\nEach letter is shifted to the right in the alphabet by a non-negative integer shift. If the shift moves past Z, continue from A.\n\nReturn the encrypted list.",
      "example": "[C, N, R], shift = 1 → [D, O, S]\n[C, N, R], shift = 3 → [F, Q, U]\n[C, N, R], shift = 26 → [C, N, R]",
      "functionName": "CaesarCipher",
      "variables": [
          "alphabet",
          "letters",
          "shift"
      ],
      "testCases": [
          {
              "inputs": [
                  "\"ABCDEFGHIJKLMNOPQRSTUVWXYZ\"",
                  "[\"C\",\"N\",\"R\"]",
                  "1"
              ],
              "output": "D,O,S"
          },
          {
              "inputs": [
                  "\"ABCDEFGHIJKLMNOPQRSTUVWXYZ\"",
                  "[\"C\",\"N\",\"R\"]",
                  "3"
              ],
              "output": "F,Q,U"
          },
          {
              "inputs": [
                  "\"ABCDEFGHIJKLMNOPQRSTUVWXYZ\"",
                  "[\"C\",\"N\",\"R\"]",
                  "26"
              ],
              "output": "C,N,R"
          },
          {
              "inputs": [
                  "\"ABCDEFGHIJKLMNOPQRSTUVWXYZ\"",
                  "[\"X\",\"Y\",\"Z\"]",
                  "3"
              ],
              "output": "A,B,C"
          },
          {
              "inputs": [
                  "\"ABCDEFGHIJKLMNOPQRSTUVWXYZ\"",
                  "[\"C\",\"N\",\"R\"]",
                  "5"
              ],
              "output": "H,S,W"
          }
      ]
  },
  {
      "id": "abundant_number",
      "title": "Task 2: Abundant Number",
      "description": "Determine whether a non-negative integer is an abundant number.\n\nA number is abundant when the sum of its proper divisors, excluding the number itself, is greater than the number.\n\nReturn true if the number is abundant and false otherwise.",
        "example": "n = 12 → true\nProper divisors: 1, 2, 3, 4, 6\n1 + 2 + 3 + 4 + 6 = 16 > 12",
      "functionName": "IsAbundant",
      "variables": [
          "n"
      ],
      "testCases": [
          {
              "inputs": [
                  "12"
              ],
              "output": "true"
          },
          {
              "inputs": [
                  "6"
              ],
              "output": "false"
          },
          {
              "inputs": [
                  "18"
              ],
              "output": "true"
          },
          {
              "inputs": [
                  "20"
              ],
              "output": "true"
          },
          {
              "inputs": [
                  "28"
              ],
              "output": "false"
          }
      ]
  },
  {
      "id": "rle_decompression",
      "title": "Task 3: RLE Decompression",
      "description": "Decompress a non-empty list encoded using Run-Length Encoding (RLE).\n\nThe list contains pairs consisting of a binary value (0 or 1) followed by the number of times that value should be repeated.\n\nReturn the decompressed list.",
        "example": "data = [0, 3, 1, 2, 0, 5, 1, 3, 0, 1] → [0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0]",
      "functionName": "RLEDecompress",
      "variables": [
          "data"
      ],
      "testCases": [
          {
              "inputs": [
                  "[0,5]"
              ],
              "output": "0,0,0,0,0"
          },
          {
              "inputs": [
                  "[1,5]"
              ],
              "output": "1,1,1,1,1"
          },
          {
              "inputs": [
                  "[0,3,1,2,0,5,1,3,0,1]"
              ],
              "output": "0,0,0,1,1,0,0,0,0,0,1,1,1,0"
          },
          {
              "inputs": [
                  "[1,1,0,2]"
              ],
              "output": "1,0,0"
          },
          {
              "inputs": [
                  "[0,2,1,3]"
              ],
              "output": "0,0,1,1,1"
          }
      ]
  },
  {
      "id": "star_out",
      "title": "Task 4: StarOut",
      "description": "Return a copy of a string where every * and any character directly next to a * are removed.",
      "functionName": "StarOut",
        "example": "a = \"ab*cd\" → \"ad\"\na = \"ab**cd\" → \"ad\"",
      "variables": [
          "a"
      ],
      "testCases": [
          {
              "inputs": [
                  "\"ab\""
              ],
              "output": "ab"
          },
          {
              "inputs": [
                  "\"ab*cd\""
              ],
              "output": "ad"
          },
          {
              "inputs": [
                  "\"ab**cd\""
              ],
              "output": "ad"
          },
          {
              "inputs": [
                  "\"sm*eilly\""
              ],
              "output": "silly"
          },
          {
              "inputs": [
                  "\"sm*eil*ly\""
              ],
              "output": "siy"
          },
          {
              "inputs": [
                  "\"*stringy\""
              ],
              "output": "tringy"
          },
          {
              "inputs": [
                  "\"a*b\""
              ],
              "output": ""
          },
          {
              "inputs": [
                  "\"a*\""
              ],
              "output": ""
          },
          {
              "inputs": [
                  "\"*a\""
              ],
              "output": ""
          },
          {
              "inputs": [
                  "\"abc\""
              ],
              "output": "abc"
          }
      ]
  },
  {
      "id": "centered_average",
      "title": "Task 5: Centered Average",
      "description": "Calculate the centered average of an integer array by ignoring one occurrence of its smallest value and one occurrence of its largest value.\n\nIf the smallest or largest value appears multiple times, ignore only one occurrence of each. Use integer division for the final average.\n\nThe array contains at least three values.",
        "example": "nums = [1, 2, 3, 4, 100] → 3",
      "functionName": "CenteredAverage",
      "variables": [
          "nums"
      ],
      "testCases": [
          {
              "inputs": [
                  "[1,2,3,4,100]"
              ],
              "output": "3"
          },
          {
              "inputs": [
                  "[1,1,5,5,10,8,7]"
              ],
              "output": "5"
          },
          {
              "inputs": [
                  "[-10,-4,-2,-4,-2,0]"
              ],
              "output": "-3"
          },
          {
              "inputs": [
                  "[5,3,4,6,2]"
              ],
              "output": "4"
          },
          {
              "inputs": [
                  "[5,3,4,0,100]"
              ],
              "output": "4"
          },
          {
              "inputs": [
                  "[1,1,100]"
              ],
              "output": "1"
          },
          {
              "inputs": [
                  "[7,7,7]"
              ],
              "output": "7"
          }
      ]
  },
  {
      "id": "not_alone",
      "title": "Task 6: NotAlone",
      "description": "An occurrence of a value in an array is considered alone when it has values on both sides and neither neighbor is equal to it.\n\nReplace every alone occurrence of the specified value with the larger of its left and right neighbors.\n\nReturn the modified array.",
        "example": "nums = [1, 2, 3], value = 2 → [1, 3, 3]",
      "functionName": "NotAlone",
      "variables": [
          "nums",
          "value"
      ],
      "testCases": [
          {
              "inputs": [
                  "[1,2,3]",
                  "2"
              ],
              "output": "1,3,3"
          },
          {
              "inputs": [
                  "[1,2,3,2,5,2]",
                  "2"
              ],
              "output": "1,3,3,5,5,2"
          },
          {
              "inputs": [
                  "[3,4]",
                  "3"
              ],
              "output": "3,4"
          },
          {
              "inputs": [
                  "[3,3]",
                  "3"
              ],
              "output": "3,3"
          },
          {
              "inputs": [
                  "[1,3,1,2]",
                  "1"
              ],
              "output": "1,3,3,2"
          },
          {
              "inputs": [
                  "[7,1,6]",
                  "1"
              ],
              "output": "7,7,6"
          },
          {
              "inputs": [
                  "[1,1,1]",
                  "1"
              ],
              "output": "1,1,1"
          }
      ]
  },
  {
      "id": "linear_in",
      "title": "Task 7: LinearIn",
      "description": "Given two integer arrays, outer and inner, sorted in increasing order, determine whether every value in inner also appears in outer.\n\nTake advantage of the sorted arrays to solve the problem using a single linear pass.\n\nReturn true if all values in inner appear in outer, and false otherwise.",
        "example": "outer = [1, 2, 4, 6], inner = [2, 4] → true\nouter = [1, 2, 4, 6], inner = [2, 3] → false",
      "functionName": "LinearIn",
      "variables": [
          "outer",
          "inner"
      ],
      "testCases": [
          {
              "inputs": [
                  "[1,2,4,6]",
                  "[2,4]"
              ],
              "output": "true"
          },
          {
              "inputs": [
                  "[1,2,4,6]",
                  "[2,3,4]"
              ],
              "output": "false"
          },
          {
              "inputs": [
                  "[1,2,4,4,6]",
                  "[2,4]"
              ],
              "output": "true"
          },
          {
              "inputs": [
                  "[2,2,4,4,6,6]",
                  "[2,4]"
              ],
              "output": "true"
          },
          {
              "inputs": [
                  "[2,2,2,2,2]",
                  "[2,2]"
              ],
              "output": "true"
          },
          {
              "inputs": [
                  "[2,2,2,2,2]",
                  "[2,4]"
              ],
              "output": "false"
          },
          {
              "inputs": [
                  "[2,2,2,2,4]",
                  "[2,4]"
              ],
              "output": "true"
          },
          {
              "inputs": [
                  "[1,2,3]",
                  "[2]"
              ],
              "output": "true"
          },
          {
              "inputs": [
                  "[1,2,3]",
                  "[-1]"
              ],
              "output": "false"
          },
          {
              "inputs": [
                  "[-1,0,3,3,3,10,12]",
                  "[-1,0,3,12]"
              ],
              "output": "true"
          }
      ]
  },
  {
      id: "all_task_completed",
      title: "Done",
      description: "All tasks are completed!",
      example: "",
      functionName: "NAN",
      variables: [],
      testCases: [
          {
          inputs: [
                  "NAN"
              ],
          output: "nan",
          },
      ],
  },
];

export const oldTasks: Task[] = [
  {
    id: "welcome_task_1",
    title: "Task 1: Welcome",
    description:
      "The function 'SayHi' should return 'Hello World'. Do this by connecting the exiting blocks in front of you.",
    functionName: "SayHi",
    example: "",
    variables: [],
    testCases: [
      {
        inputs: [],
        output: "Hello World",
      },
    ],
  },
  // {
  //     id: "welcome_task_2",
  //     title: "Task 1: Welcome Code",
  //     description: "Now, the function 'SayCode' should return 'Hello Code'. Do this by editing the text.",
  //     functionName: "SayCode",
  //     variables: [],
  //     testCases: [
  //         {
  //             inputs: [],
  //             output: "Hello Code"
  //         }
  //     ]
  // },
  {
    id: "math_task_1",
    title: "Task 2: Simple multiplication",
    description:
      "The function 'SolveMathProblem' should return the value of 2 x 3. Do this by using the arithmetic block and number blocks in front of you.",
    functionName: "SolveMathProblem",
    example: "",
    variables: [],
    testCases: [
      {
        inputs: [],
        output: "6",
      },
    ],
  },
  {
    id: "math_task_2",
    title: "Task 3: Sum of two variables",
    description:
      "The function 'SolveMathProblem' should return the sum of variables 'X' and 'Y'.",
    example: "",
    functionName: "SolveMathProblem",
    variables: ["x", "y"],
    testCases: [
      {
        inputs: ["-1", "1"],
        output: "0",
      },
      {
        inputs: ["0", "0"],
        output: "0",
      },
      {
        inputs: ["4", "6"],
        output: "10",
      },
    ],
  },
  {
    id: "math_task_3",
    title: "Task 4: Add new blocks",
    description:
      "The function 'SolveMathProblem' should return the multiplication of the variables 'X' and 'Y'. You need to add the needed blocks from the menu to the left.",
    example: "",
    functionName: "SolveMathProblem",
    variables: ["x", "y"],
    testCases: [
      {
        inputs: ["-1", "1"],
        output: "-1",
      },
      {
        inputs: ["0", "0"],
        output: "0",
      },
      {
        inputs: ["4", "6"],
        output: "24",
      },
    ],
  },
  {
    id: "age_bug",
    title: "Task 5: Bug in code",
    description:
      "The function 'IsAllowedPension' should return true if the given age is greater or equal to 62. The code starts out faulty. Can you fix it?",
    example: "",
    functionName: "IsAllowedPension",
    variables: ["age"],
    testCases: [
      {
        inputs: ["-1"],
        output: "false",
      },
      {
        inputs: ["61"],
        output: "false",
      },
      {
        inputs: ["26"],
        output: "false",
      },
      {
        inputs: ["62"],
        output: "true",
      },
      {
        inputs: ["78"],
        output: "true",
      },
    ],
  },
  {
    id: "match_correct_function_task",
    title: "Task 6: Match number and function",
    description:
      "The function 'MakeMeTrue' should return true. Do this by putting the correct function call with the correct number. For example, a function that return 6 should be matched with the number 6.",
    example: "",
    functionName: "MakeMeTrue",
    variables: [],
    testCases: [
      {
        inputs: [],
        output: "true",
      },
    ],
  },
  {
    id: "order_numbers_task",
    title: "Task 7: Order numbers",
    description:
      "In the function 'OrderNumbers', move the number blocks such that they are in acceding order. That is, 0 1 2 3 4... Remember to only move the number blocks! You are not allowed to edit the input.",
    example: "",
    functionName: "OrderNumbers",
    variables: [],
    testCases: [
      {
        inputs: [],
        output: "26",
      },
    ],
  },
  // {
  //     id: "drive_task",
  //     title: "Task 1: Allowed to drive checker",
  //     description: "Given the age and the drunk state of a person, the function 'canDrive' should return true if the person can drive and false if not.",
  //     functionName: "canDrive",
  //     variables: ["age", "isDrunk"],
  //     testCases: [
  //         {
  //             inputs: ["10", "false"],
  //             output: "false"
  //         },
  //         {
  //             inputs: ["19", "true"],
  //             output: "false"
  //         },
  //         {
  //             inputs: ["20", "false"],
  //             output: "true"
  //         }
  //     ]
  // },
  // {
  //     id: "sum_task",
  //     title: "Task 2: Add numbers",
  //     description: "Given two numbers (x, y) the sum should be returned in the function 'addTwoNumbers'",
  //     functionName: "addTwoNumbers",
  //     variables: ["x", "y"],
  //     testCases: [
  //         {
  //             inputs: ["1", "1"],
  //             output: "2"
  //         },
  //         {
  //             inputs: ["-5", "5"],
  //             output: "0"
  //         },
  //         {
  //             inputs: ["0", "0"],
  //             output: "0"
  //         }
  //     ]
  // },
  {
    id: "area_task",
    title: "Task 8: Calculate Area",
    description:
      "The function 'CalculateArea' should return the area of a rectangle with the sides 'X' and 'Y'. However, if any of the sides are less than zero, the function should return 0",
    example: "",
    functionName: "CalculateArea",
    variables: ["x", "y"],
    testCases: [
      {
        inputs: ["-1", "1"],
        output: "0",
      },
      {
        inputs: ["0", "0"],
        output: "0",
      },
      {
        inputs: ["1", "-1"],
        output: "0",
      },
      {
        inputs: ["2", "3"],
        output: "6",
      },
    ],
  },
  {
    id: "create_function_task",
    title: "Task 9: Create Function",
    description:
      "The function 'CallFunction' should call a new function that you need to create. The new function should be called 'foo' and have the inputs 'a' and 'b'. It should only return true if a > b, otherwise false. Call the function using x as parameter a and y as parameter b.",
    example: "",
    functionName: "CallFunction",
    variables: ["x", "y"],
    testCases: [
      {
        inputs: ["3", "1"],
        output: "true",
      },
      {
        inputs: ["1", "1"],
        output: "false",
      },
      {
        inputs: ["4", "9"],
        output: "false",
      },
    ],
  },
  {
    id: "all_task_completed",
    title: "Done",
    description: "All tasks are completed!",
    example: "",
    functionName: "NAN",
    variables: [],
    testCases: [
      {
        inputs: ["NAN"],
        output: "nan",
      },
    ],
  },
];


export default Task;
