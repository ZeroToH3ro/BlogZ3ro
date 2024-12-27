-- Create Database
-- CREATE
--     DATABASE blogdb;

-- \c blogdb

-- Create Schema
CREATE SCHEMA aggregate;

-- Note: PostgreSQL doesn't have TABLE types like SQL Server
-- We'll skip the custom types (AccountType, BlogCommentType, etc.) as they're typically
-- handled differently in PostgreSQL

-- Create Tables
CREATE TABLE application_user
(
    application_user_id SERIAL PRIMARY KEY,
    username            VARCHAR(20) NOT NULL,
    normalized_username VARCHAR(20) NOT NULL,
    email               VARCHAR(30) NOT NULL,
    normalized_email    VARCHAR(30) NOT NULL,
    fullname            VARCHAR(30),
    password_hash       TEXT        NOT NULL
);

CREATE INDEX ix_application_user_normalized_username
    ON application_user (normalized_username);

CREATE INDEX ix_application_user_normalized_email
    ON application_user (normalized_email);

CREATE TABLE photo
(
    photo_id            SERIAL PRIMARY KEY,
    application_user_id INTEGER      NOT NULL,
    public_id           VARCHAR(50)  NOT NULL,
    image_url           VARCHAR(250) NOT NULL,
    description         VARCHAR(30)  NOT NULL,
    publish_date        TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_date         TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (application_user_id) REFERENCES application_user (application_user_id)
);

CREATE TABLE blog
(
    blog_id             SERIAL PRIMARY KEY,
    application_user_id INTEGER     NOT NULL,
    photo_id            INTEGER,
    title               VARCHAR(50) NOT NULL,
    content             TEXT        NOT NULL,
    publish_date        TIMESTAMP   NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_date         TIMESTAMP   NOT NULL DEFAULT CURRENT_TIMESTAMP,
    active_ind          BOOLEAN     NOT NULL DEFAULT TRUE,
    FOREIGN KEY (application_user_id) REFERENCES application_user (application_user_id),
    FOREIGN KEY (photo_id) REFERENCES photo (photo_id)
);

CREATE TABLE blog_comment
(
    blog_comment_id        SERIAL PRIMARY KEY,
    parent_blog_comment_id INTEGER,
    blog_id                INTEGER      NOT NULL,
    application_user_id    INTEGER      NOT NULL,
    content                VARCHAR(300) NOT NULL,
    publish_date           TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_date            TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    active_ind             BOOLEAN      NOT NULL DEFAULT TRUE,
    FOREIGN KEY (blog_id) REFERENCES blog (blog_id),
    FOREIGN KEY (application_user_id) REFERENCES application_user (application_user_id)
);

-- Create Views
CREATE VIEW aggregate.blog AS
SELECT t1.blog_id,
       t1.application_user_id,
       t2.username,
       t1.title,
       t1.content,
       t1.photo_id,
       t1.publish_date,
       t1.update_date,
       t1.active_ind
FROM blog t1
         INNER JOIN
     application_user t2 ON t1.application_user_id = t2.application_user_id;

CREATE VIEW aggregate.blog_comment AS
SELECT t1.blog_comment_id,
       t1.parent_blog_comment_id,
       t1.blog_id,
       t1.content,
       t2.username,
       t1.application_user_id,
       t1.publish_date,
       t1.update_date,
       t1.active_ind
FROM blog_comment t1
         INNER JOIN
     application_user t2 ON t1.application_user_id = t2.application_user_id;

CREATE OR REPLACE FUNCTION account_get_by_username(p_normalized_username VARCHAR(20))
    RETURNS TABLE
            (
                application_user_id INTEGER,
                username            VARCHAR(20),
                normalized_username VARCHAR(20),
                email               VARCHAR(30),
                normalized_email    VARCHAR(30),
                fullname            VARCHAR(30),
                password_hash       TEXT
            )
AS
$BODY$
BEGIN
    RETURN QUERY
        SELECT au.application_user_id,
               au.username,
               au.normalized_username,
               au.email,
               au.normalized_email,
               au.fullname,
               au.password_hash
        FROM application_user au
        WHERE au.normalized_username = p_normalized_username;
END;
$BODY$
    LANGUAGE plpgsql;