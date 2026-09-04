using System.Text;

namespace SAFTLang.CodeGenerator.Runtime;

internal static class RuntimeGenerator
{
    public static void GenerateRuntime(StringBuilder output)
    {
        output.AppendLine(
            """
            /*
             * ============================================================
             * SAFT Runtime
             * ============================================================
             */

            typedef bool (*saft_equals_fn)(
                const void* left,
                const void* right
            );

            typedef void (*saft_retain_fn)(
                void* value
            );

            typedef void (*saft_release_fn)(
                void* value
            );


            /*
             * ============================================================
             * Array
             * ============================================================
             */

            typedef struct
            {
                void* data;

                size_t length;
                size_t capacity;
                size_t element_size;

                size_t ref_count;

                saft_equals_fn equals;

                saft_retain_fn retain_element;
                saft_release_fn release_element;

            } saft_array_storage;


            typedef struct
            {
                saft_array_storage* storage;

            } saft_array;


            /*
             * ============================================================
             * Forward declarations
             * ============================================================
             */

            static bool saft_array_equal(
                saft_array left,
                saft_array right
            );

            static saft_array saft_array_retain(
                saft_array array
            );

            static void saft_array_release(
                saft_array array
            );

            static void saft_retain_array_element(
                void* value
            );

            static void saft_release_array_element(
                void* value
            );


            /*
             * ============================================================
             * Runtime errors
             * ============================================================
             */

            static void saft_runtime_error(
                const char* message
            )
            {
                fprintf(
                    stderr,
                    "SAFT runtime error: %s\n",
                    message
                );

                exit(1);
            }


            static void saft_out_of_memory(void)
            {
                saft_runtime_error(
                    "out of memory"
                );
            }


            static void saft_array_require_valid(
                saft_array array
            )
            {
                if (array.storage == NULL)
                {
                    saft_runtime_error(
                        "invalid array"
                    );
                }
            }


            /*
             * ============================================================
             * Equality functions
             * ============================================================
             */

            static bool saft_equal_int(
                const void* left,
                const void* right
            )
            {
                return
                    *(const int*)left ==
                    *(const int*)right;
            }


            static bool saft_equal_bool(
                const void* left,
                const void* right
            )
            {
                return
                    *(const bool*)left ==
                    *(const bool*)right;
            }


            static bool saft_equal_string(
                const void* left,
                const void* right
            )
            {
                const char* left_value =
                    *(const char* const*)left;

                const char* right_value =
                    *(const char* const*)right;

                if (
                    left_value == NULL ||
                    right_value == NULL
                )
                {
                    return left_value == right_value;
                }

                return strcmp(
                    left_value,
                    right_value
                ) == 0;
            }


            static bool saft_equal_array(
                const void* left,
                const void* right
            )
            {
                return saft_array_equal(
                    *(const saft_array*)left,
                    *(const saft_array*)right
                );
            }


            /*
             * ============================================================
             * Array storage creation
             * ============================================================
             */

            static saft_array saft_array_create_managed(
                size_t element_size,
                size_t capacity,
                saft_equals_fn equals,
                saft_retain_fn retain_element,
                saft_release_fn release_element
            )
            {
                if (element_size == 0)
                {
                    saft_runtime_error(
                        "array element size cannot be zero"
                    );
                }

                saft_array_storage* storage =
                    malloc(sizeof(saft_array_storage));

                if (storage == NULL)
                {
                    saft_out_of_memory();
                }

                storage->data = NULL;

                storage->length = 0;
                storage->capacity = 0;

                storage->element_size =
                    element_size;

                storage->ref_count = 1;

                storage->equals =
                    equals;

                storage->retain_element =
                    retain_element;

                storage->release_element =
                    release_element;


                if (capacity > 0)
                {
                    if (
                        capacity >
                        (size_t)-1 / element_size
                    )
                    {
                        free(storage);

                        saft_runtime_error(
                            "array allocation too large"
                        );
                    }

                    storage->data = malloc(
                        capacity * element_size
                    );

                    if (storage->data == NULL)
                    {
                        free(storage);

                        saft_out_of_memory();
                    }

                    storage->capacity =
                        capacity;
                }


                saft_array result =
                {
                    storage
                };

                return result;
            }


            /*
             * Normal array creation.
             *
             * Currently used mainly for empty arrays.
             */

            static saft_array saft_array_create(
                size_t element_size,
                saft_equals_fn equals
            )
            {
                saft_retain_fn retain_element =
                    NULL;

                saft_release_fn release_element =
                    NULL;

                /*
                 * Arrays inside arrays need reference counting.
                 *
                 * This keeps the current code generator compatible.
                 * Later the compiler can explicitly pass the correct
                 * retain/release functions for every managed type.
                 */

                if (equals == saft_equal_array)
                {
                    retain_element =
                        saft_retain_array_element;

                    release_element =
                        saft_release_array_element;
                }

                return saft_array_create_managed(
                    element_size,
                    0,
                    equals,
                    retain_element,
                    release_element
                );
            }


            /*
             * ============================================================
             * Reference counting
             * ============================================================
             */

            static saft_array saft_array_retain(
                saft_array array
            )
            {
                if (array.storage != NULL)
                {
                    array.storage->ref_count++;
                }

                return array;
            }


            static void saft_array_release(
                saft_array array
            )
            {
                if (array.storage == NULL)
                {
                    return;
                }

                if (array.storage->ref_count == 0)
                {
                    saft_runtime_error(
                        "invalid array reference count"
                    );
                }

                array.storage->ref_count--;

                if (array.storage->ref_count != 0)
                {
                    return;
                }


                if (
                    array.storage->release_element != NULL
                )
                {
                    for (
                        size_t i = 0;
                        i < array.storage->length;
                        i++
                    )
                    {
                        void* element =
                            (char*)array.storage->data +
                            i *
                            array.storage->element_size;

                        array.storage->release_element(
                            element
                        );
                    }
                }


                free(
                    array.storage->data
                );

                free(
                    array.storage
                );
            }


            static void saft_retain_array_element(
                void* value
            )
            {
                saft_array* array =
                    (saft_array*)value;

                saft_array_retain(
                    *array
                );
            }


            static void saft_release_array_element(
                void* value
            )
            {
                saft_array* array =
                    (saft_array*)value;

                saft_array_release(
                    *array
                );
            }


            /*
             * Safe assignment helper.
             *
             * Example:
             *
             *     saft_array_assign(&a, b);
             *
             * instead of:
             *
             *     a = b;
             */

            static void saft_array_assign(
                saft_array* target,
                saft_array source
            )
            {
                saft_array retained =
                    saft_array_retain(
                        source
                    );

                saft_array_release(
                    *target
                );

                *target =
                    retained;
            }


            /*
             * ============================================================
             * Array reserve / capacity
             * ============================================================
             */

            static void saft_array_reserve(
                saft_array array,
                size_t capacity
            )
            {
                saft_array_require_valid(
                    array
                );

                saft_array_storage* storage =
                    array.storage;


                if (capacity <= storage->capacity)
                {
                    return;
                }


                if (
                    capacity >
                    (size_t)-1 /
                    storage->element_size
                )
                {
                    saft_runtime_error(
                        "array allocation too large"
                    );
                }


                void* new_data =
                    realloc(
                        storage->data,
                        capacity *
                        storage->element_size
                    );


                if (new_data == NULL)
                {
                    saft_out_of_memory();
                }


                storage->data =
                    new_data;

                storage->capacity =
                    capacity;
            }


            /*
             * ============================================================
             * Array copy
             * ============================================================
             */

            static saft_array saft_array_copy_managed(
                const void* source,
                size_t element_size,
                size_t length,
                saft_equals_fn equals,
                saft_retain_fn retain_element,
                saft_release_fn release_element
            )
            {
                if (
                    length > 0 &&
                    source == NULL
                )
                {
                    saft_runtime_error(
                        "cannot create array from null source"
                    );
                }


                saft_array result =
                    saft_array_create_managed(
                        element_size,
                        length,
                        equals,
                        retain_element,
                        release_element
                    );


                if (length == 0)
                {
                    return result;
                }


                memcpy(
                    result.storage->data,
                    source,
                    element_size * length
                );


                result.storage->length =
                    length;


                /*
                 * Retain referenced objects.
                 *
                 * Primitive elements such as int/bool have NULL here.
                 */

                if (retain_element != NULL)
                {
                    for (
                        size_t i = 0;
                        i < length;
                        i++
                    )
                    {
                        void* element =
                            (char*)result.storage->data +
                            i * element_size;

                        retain_element(
                            element
                        );
                    }
                }


                return result;
            }


            /*
             * Compatibility function for the current compiler.
             *
             * Your current GenerateArrayExpression() can keep calling:
             *
             * saft_array_copy(source, sizeof(T), count, equals)
             */

            static saft_array saft_array_copy(
                const void* source,
                size_t element_size,
                int length,
                saft_equals_fn equals
            )
            {
                if (length < 0)
                {
                    saft_runtime_error(
                        "array length cannot be negative"
                    );
                }


                saft_retain_fn retain_element =
                    NULL;

                saft_release_fn release_element =
                    NULL;


                if (equals == saft_equal_array)
                {
                    retain_element =
                        saft_retain_array_element;

                    release_element =
                        saft_release_array_element;
                }


                return saft_array_copy_managed(
                    source,
                    element_size,
                    (size_t)length,
                    equals,
                    retain_element,
                    release_element
                );
            }


            /*
             * ============================================================
             * Array information
             * ============================================================
             */

            static size_t saft_array_len(
                saft_array array
            )
            {
                saft_array_require_valid(
                    array
                );

                return
                    array.storage->length;
            }


            static size_t saft_array_capacity(
                saft_array array
            )
            {
                saft_array_require_valid(
                    array
                );

                return
                    array.storage->capacity;
            }


            /*
             * ============================================================
             * Array indexing
             * ============================================================
             */

            static void* saft_array_at(
                saft_array array,
                int index
            )
            {
                saft_array_require_valid(
                    array
                );


                if (
                    index < 0 ||
                    (size_t)index >=
                    array.storage->length
                )
                {
                    saft_runtime_error(
                        "array index out of bounds"
                    );
                }


                return
                    (char*)array.storage->data +
                    (size_t)index *
                    array.storage->element_size;
            }


            /*
             * ============================================================
             * Array append
             * ============================================================
             */

            static void saft_array_append(
                saft_array array,
                const void* value
            )
            {
                saft_array_require_valid(
                    array
                );


                if (value == NULL)
                {
                    saft_runtime_error(
                        "cannot append null element pointer"
                    );
                }


                saft_array_storage* storage =
                    array.storage;


                /*
                 * If realloc is required, value might point into
                 * the array itself:
                 *
                 *     arr.append(arr[0])
                 *
                 * Therefore copy it temporarily before realloc.
                 */

                void* temporary =
                    NULL;


                if (
                    storage->length ==
                    storage->capacity
                )
                {
                    temporary =
                        malloc(
                            storage->element_size
                        );

                    if (temporary == NULL)
                    {
                        saft_out_of_memory();
                    }


                    memcpy(
                        temporary,
                        value,
                        storage->element_size
                    );


                    size_t new_capacity;

                    if (storage->capacity == 0)
                    {
                        new_capacity = 4;
                    }
                    else
                    {
                        if (
                            storage->capacity >
                            (size_t)-1 / 2
                        )
                        {
                            free(
                                temporary
                            );

                            saft_runtime_error(
                                "array too large"
                            );
                        }

                        new_capacity =
                            storage->capacity * 2;
                    }


                    saft_array_reserve(
                        array,
                        new_capacity
                    );


                    value =
                        temporary;
                }


                void* destination =
                    (char*)storage->data +
                    storage->length *
                    storage->element_size;


                /*
                 * memmove instead of memcpy allows values from
                 * inside the same array.
                 */

                memmove(
                    destination,
                    value,
                    storage->element_size
                );


                /*
                 * If the element owns another runtime object
                 * (currently mainly nested arrays), retain it.
                 */

                if (
                    storage->retain_element != NULL
                )
                {
                    storage->retain_element(
                        destination
                    );
                }


                storage->length++;


                free(
                    temporary
                );
            }


            /*
             * ============================================================
             * Safe indexed assignment
             * ============================================================
             *
             * This becomes useful once reference-counted elements
             * such as nested arrays can be replaced.
             */

            static void saft_array_set(
                saft_array array,
                int index,
                const void* value
            )
            {
                saft_array_require_valid(
                    array
                );


                if (value == NULL)
                {
                    saft_runtime_error(
                        "cannot assign null element pointer"
                    );
                }


                void* destination =
                    saft_array_at(
                        array,
                        index
                    );


                size_t element_size =
                    array.storage->element_size;


                /*
                 * Temporary copy makes self-assignment safe:
                 *
                 *     arr[0] = arr[0]
                 */

                void* temporary =
                    malloc(
                        element_size
                    );


                if (temporary == NULL)
                {
                    saft_out_of_memory();
                }


                memcpy(
                    temporary,
                    value,
                    element_size
                );


                /*
                 * Retain new value before releasing old value.
                 */

                if (
                    array.storage->retain_element != NULL
                )
                {
                    array.storage->retain_element(
                        temporary
                    );
                }


                if (
                    array.storage->release_element != NULL
                )
                {
                    array.storage->release_element(
                        destination
                    );
                }


                memcpy(
                    destination,
                    temporary,
                    element_size
                );


                free(
                    temporary
                );
            }


            /*
             * ============================================================
             * Array equality
             * ============================================================
             */

            static bool saft_array_equal(
                saft_array left,
                saft_array right
            )
            {
                /*
                 * Same runtime object.
                 */

                if (
                    left.storage ==
                    right.storage
                )
                {
                    return true;
                }


                if (
                    left.storage == NULL ||
                    right.storage == NULL
                )
                {
                    return false;
                }


                if (
                    left.storage->length !=
                    right.storage->length
                )
                {
                    return false;
                }


                if (
                    left.storage->element_size !=
                    right.storage->element_size
                )
                {
                    return false;
                }


                if (
                    left.storage->equals !=
                    right.storage->equals
                )
                {
                    return false;
                }


                if (
                    left.storage->equals == NULL
                )
                {
                    return false;
                }


                for (
                    size_t i = 0;
                    i < left.storage->length;
                    i++
                )
                {
                    const void* left_element =
                        (const char*)left.storage->data +
                        i *
                        left.storage->element_size;


                    const void* right_element =
                        (const char*)right.storage->data +
                        i *
                        right.storage->element_size;


                    if (
                        !left.storage->equals(
                            left_element,
                            right_element
                        )
                    )
                    {
                        return false;
                    }
                }


                return true;
            }

            """
        );
    }
}