import { Component, Input } from '@angular/core';

export type TagItem = string | { name: string };

@Component({
  selector: 'app-tags',
  templateUrl: './tags.component.html',
  styleUrls: ['./tags.component.scss']
})
export class TagsComponent {
  private readonly VISIBLE_TAGS_LIMIT = 2;
  private cachedTooltipContent = '';
  private cachedHiddenTagsLength = 0;

  @Input() items: TagItem[] = [];
  @Input() expanded = false;
  /**
   * Layout direction of tags. Default is 'row'.
   * When set to 'column' each tag is rendered on its own line.
   */
  @Input() direction: 'row' | 'column' = 'row';

  /**
   * Normalizes the provided items into their displayable string representation.
   */
  private get tagNames(): string[] {
    return this.items.map((item) => (typeof item === 'string' ? item : item?.name ?? ''));
  }

  get visibleTags(): string[] {
    return this.expanded ? this.tagNames : this.tagNames.slice(0, this.VISIBLE_TAGS_LIMIT);
  }

  get hiddenTags(): string[] {
    return this.expanded ? [] : this.tagNames.slice(this.VISIBLE_TAGS_LIMIT);
  }

  get hiddenCount(): number {
    return this.hiddenTags.length;
  }

  get hiddenTagsTooltip(): string {
    if (this.hiddenTags.length !== this.cachedHiddenTagsLength) {
      this.cachedHiddenTagsLength = this.hiddenTags.length;
      this.cachedTooltipContent = this.hiddenTags.join(', ');
    }

    return this.cachedTooltipContent;
  }
} 