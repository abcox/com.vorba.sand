export * from './blog.service';
import { BlogService } from './blog.service';
export * from './post.service';
import { PostService } from './post.service';
export * from './test.service';
import { TestService } from './test.service';
export const APIS = [BlogService, PostService, TestService];
