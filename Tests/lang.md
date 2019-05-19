# Custom function coordinator language

## Simple rules
- If args not set, assumed to be "$"
- If output not set, assumed to be "$"
- Spread with ...$, destructuring with [varOne,] or { prop: varOne }

## Sample

```js
next: {
    invoke: "some.arn.to.something"
    output: {
        ...$, //Spread whole output
        custom_prop: 5
    }
    next: [ next_step1, next_step2 ]
    error: stop

    next_step1: {
        invoke: "some.other.arn"
        args: {
            data: $, // Assign output of last input
            namespace: "build_path"
        }
    }

    next_step2: {
        invoke: "some.other.arn"
        args: {
            data: $, // Assign output of last input
            namespace: "build_path"
        }
        next: {
            if: $.value = "changed"
            next: log_data
        }
    }

    log_data: {
        invoke: "some.other.arn"
    }
}
```