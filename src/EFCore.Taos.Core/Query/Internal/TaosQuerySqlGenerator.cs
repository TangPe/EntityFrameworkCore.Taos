// Copyright (c)  Maikebing. All rights reserved.
// Licensed under the MIT License, See License.txt in the project root for license information.

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Maikebing.EntityFrameworkCore.Taos.Query.Internal
{
    public class TaosQuerySqlGenerator : QuerySqlGenerator
    {
        public TaosQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies)
            : base(dependencies)
        {
        }

        protected override string GenerateOperator(SqlBinaryExpression binaryExpression)
            => binaryExpression.OperatorType == ExpressionType.Add
                && binaryExpression.Type == typeof(string)
                    ? " || "
                    : base.GenerateOperator(binaryExpression);

        protected override void GenerateLimitOffset(SelectExpression selectExpression)
        {
            Check.NotNull(selectExpression, nameof(selectExpression));

            if (selectExpression.Limit != null
                || selectExpression.Offset != null)
            {
                Sql.AppendLine()
                    .Append("LIMIT ");

                Visit(
                    selectExpression.Limit
                    ?? new SqlConstantExpression(Expression.Constant(-1), selectExpression.Offset.TypeMapping));

                if (selectExpression.Offset != null)
                {
                    Sql.Append(" OFFSET ");

                    Visit(selectExpression.Offset);
                }
            }
        }

        protected override void GenerateSetOperationOperand(SetOperationBase setOperation, SelectExpression operand)
        {
            // Taos doesn't support parentheses around set operation operands
            Visit(operand);
        }
    }
}
