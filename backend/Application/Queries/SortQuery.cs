﻿using Domain.Shared.Enums;

namespace Application.Queries;

public class SortQuery
{
    public SortQuery()
    {
        SortField = ModelFields.None;
        SortDirection = SortDirections.Ascending;
    }

    public ModelFields SortField { get; set; }
    public SortDirections SortDirection { get; set; }
}